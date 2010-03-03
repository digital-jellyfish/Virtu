using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Jellyfish.Library
{
    public sealed class DirectSoundUpdateEventArgs : EventArgs
    {
        private DirectSoundUpdateEventArgs()
        {
        }

        public static DirectSoundUpdateEventArgs Create(IntPtr buffer, int bufferSize)
        {
            _instance.Buffer = buffer;
            _instance.BufferSize = bufferSize;

            return _instance;  // use singleton; avoids garbage
        }

        public IntPtr Buffer { get; private set; }
        public int BufferSize { get; private set; }

        private static readonly DirectSoundUpdateEventArgs _instance = new DirectSoundUpdateEventArgs();
    }

    public sealed partial class DirectSound : IDisposable
    {
        public DirectSound(int sampleRate, int sampleChannels, int sampleBits, int sampleSize)
        {
            _sampleRate = sampleRate;
            _sampleChannels = sampleChannels;
            _sampleBits = sampleBits;
            _sampleSize = sampleSize;

            _thread = new Thread(Run) { Name = "DirectSound" };
        }

        public void Dispose()
        {
            _position1Event.Close();
            _position2Event.Close();
            _stopEvent.Close();
        }

        public void Start(IntPtr window)
        {
            _window = window;
            _thread.Start();
        }

        public void Stop()
        {
            _stopEvent.Set();
            _thread.Join();
        }

        private void Initialize()
        {
            int hresult = NativeMethods.DirectSoundCreate(IntPtr.Zero, out _device, IntPtr.Zero);
            if (hresult < 0)
            {
                Marshal.ThrowExceptionForHR(hresult);
            }

            _device.SetCooperativeLevel(_window, CooperativeLevel.Normal);

            GCHandleHelpers.Pin(new WaveFormat(_sampleRate, _sampleChannels, _sampleBits), waveFormat => 
            {
                BufferDescription description = new BufferDescription(BufferCapabilities.CtrlPositionNotify | BufferCapabilities.CtrlVolume, BlockCount * _sampleSize, waveFormat);
                _device.CreateSoundBuffer(description, out _buffer, IntPtr.Zero);
            });
            ClearBuffer();

            BufferPositionNotify[] positionEvents = new BufferPositionNotify[BlockCount]
            {
                new BufferPositionNotify(0 * _sampleSize, _position1Event), new BufferPositionNotify(1 * _sampleSize, _position2Event)
            };
            ((IDirectSoundNotify)_buffer).SetNotificationPositions(positionEvents.Length, positionEvents);

            _buffer.SetVolume(-1500); // 50 %

            _buffer.Play(0, 0, BufferPlay.Looping);
        }

        private void ClearBuffer()
        {
            UpdateBuffer(0, 0, BufferLock.EntireBuffer, (buffer, bufferSize) => 
            {
                MarshalHelpers.ZeroMemory(buffer, bufferSize);
            });
        }

        private void RestoreBuffer()
        {
            BufferStatus status;
            _buffer.GetStatus(out status);
            if ((status & BufferStatus.BufferLost) != 0)
            {
                _buffer.Restore();
            }
        }

        private void UpdateBuffer(int block)
        {
            EventHandler<DirectSoundUpdateEventArgs> handler = Update;
            if (handler != null)
            {
                UpdateBuffer(block * _sampleSize, _sampleSize, BufferLock.None, (buffer, bufferSize) => 
                {
                    handler(this, DirectSoundUpdateEventArgs.Create(buffer, bufferSize));
                });
            }
        }

        private void UpdateBuffer(int offset, int count, BufferLock flags, Action<IntPtr, int> updateBuffer)
        {
            RestoreBuffer();

            IntPtr buffer1, buffer2;
            int buffer1Size, buffer2Size;
            _buffer.Lock(offset, count, out buffer1, out buffer1Size, out buffer2, out buffer2Size, flags);
            try
            {
                if (buffer1 != IntPtr.Zero)
                {
                    updateBuffer(buffer1, buffer1Size);
                }
                if (buffer2 != IntPtr.Zero)
                {
                    updateBuffer(buffer2, buffer2Size);
                }
            }
            finally
            {
                _buffer.Unlock(buffer1, buffer1Size, buffer2, buffer2Size);
            }
        }

        private void Uninitialize()
        {
            if (_buffer != null)
            {
                _buffer.Stop();
                Marshal.ReleaseComObject(_buffer);
            }
            if (_device != null)
            {
                Marshal.ReleaseComObject(_device);
            }
        }

        private void Run() // com mta thread
        {
            Initialize();

            EventWaitHandle[] eventHandles = new EventWaitHandle[] { _position1Event, _position2Event, _stopEvent };
            int index = WaitHandle.WaitAny(eventHandles);

            while (index < BlockCount)
            {
                UpdateBuffer((index + 1) % BlockCount); // update next block in circular buffer
                index = WaitHandle.WaitAny(eventHandles);
            }

            Uninitialize();
        }

        public event EventHandler<DirectSoundUpdateEventArgs> Update;

        private const int BlockCount = 2;

        private int _sampleRate;
        private int _sampleChannels;
        private int _sampleBits;
        private int _sampleSize;

        private Thread _thread;
        private IntPtr _window;
        private IDirectSound _device;
        private IDirectSoundBuffer _buffer;

        private AutoResetEvent _position1Event = new AutoResetEvent(false);
        private AutoResetEvent _position2Event = new AutoResetEvent(false);
        private ManualResetEvent _stopEvent = new ManualResetEvent(false);
    }
}
