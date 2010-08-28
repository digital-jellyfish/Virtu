using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace Jellyfish.Library
{
    public sealed partial class DirectSound : IDisposable
    {
        [SecurityCritical]
        public DirectSound(int sampleRate, int sampleChannels, int sampleBits, int sampleSize, Action<IntPtr, int> updater)
        {
            _sampleRate = sampleRate;
            _sampleChannels = sampleChannels;
            _sampleBits = sampleBits;
            _sampleSize = sampleSize;

            _thread = new Thread(Run) { Name = "DirectSound" };
            _updater = updater;
        }

        public void Dispose()
        {
            _position1Event.Close();
            _position2Event.Close();
            _stopEvent.Close();
        }

        public void SetVolume(double volume)
        {
            int attenuation = (volume < 0.01) ? (int)BufferVolume.Min : (int)Math.Floor(100 * 20 * Math.Log10(volume)); // 100 db
            lock (_bufferLock)
            {
                if (_buffer != null)
                {
                    _buffer.SetVolume(attenuation);
                }
            }
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

        [SecurityCritical]
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
                var description = new BufferDescription(BufferCapabilities.CtrlPositionNotify | BufferCapabilities.CtrlVolume, BlockCount * _sampleSize, waveFormat);
                _device.CreateSoundBuffer(description, out _buffer, IntPtr.Zero);
            });
            ClearBuffer();

            var positionEvents = new BufferPositionNotify[BlockCount]
            {
                new BufferPositionNotify(0 * _sampleSize, _position1Event), new BufferPositionNotify(1 * _sampleSize, _position2Event)
            };
            ((IDirectSoundNotify)_buffer).SetNotificationPositions(positionEvents.Length, positionEvents);

            _buffer.Play(0, 0, BufferPlay.Looping);
        }

        [SecurityCritical]
        private void ClearBuffer()
        {
            UpdateBuffer(0, 0, BufferLock.EntireBuffer, (buffer, bufferSize) => MarshalHelpers.ZeroMemory(buffer, bufferSize));
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

        private void UpdateBuffer(int offset, int count, BufferLock flags, Action<IntPtr, int> updater)
        {
            RestoreBuffer();

            IntPtr buffer1, buffer2;
            int buffer1Size, buffer2Size;
            _buffer.Lock(offset, count, out buffer1, out buffer1Size, out buffer2, out buffer2Size, flags);
            try
            {
                if (buffer1 != IntPtr.Zero)
                {
                    updater(buffer1, buffer1Size);
                }
                if (buffer2 != IntPtr.Zero)
                {
                    updater(buffer2, buffer2Size);
                }
            }
            finally
            {
                _buffer.Unlock(buffer1, buffer1Size, buffer2, buffer2Size);
            }
        }

        private void Uninitialize()
        {
            lock (_bufferLock)
            {
                if (_buffer != null)
                {
                    _buffer.Stop();
                    Marshal.ReleaseComObject(_buffer);
                    _buffer = null;
                }
            }
            if (_device != null)
            {
                Marshal.ReleaseComObject(_device);
                _device = null;
            }
        }

        [SecurityCritical]
        private void Run() // com mta thread
        {
            Initialize();

            var eventHandles = new EventWaitHandle[] { _position1Event, _position2Event, _stopEvent };
            int index = WaitHandle.WaitAny(eventHandles);

            while (index < BlockCount)
            {
                UpdateBuffer(((index + 1) % BlockCount) * _sampleSize, _sampleSize, BufferLock.None, _updater); // update next block in circular buffer
                index = WaitHandle.WaitAny(eventHandles);
            }

            Uninitialize();
        }

        private const int BlockCount = 2;

        private int _sampleRate;
        private int _sampleChannels;
        private int _sampleBits;
        private int _sampleSize;

        private Thread _thread;
        private IntPtr _window;
        private IDirectSound _device;
        private IDirectSoundBuffer _buffer;
        private object _bufferLock = new object();
        private Action<IntPtr, int> _updater;

        private AutoResetEvent _position1Event = new AutoResetEvent(false);
        private AutoResetEvent _position2Event = new AutoResetEvent(false);
        private ManualResetEvent _stopEvent = new ManualResetEvent(false);
    }
}
