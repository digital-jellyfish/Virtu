using System;
using System.Threading;

namespace Jellyfish.Virtu.Services
{
    public class AudioService : MachineService
    {
        public AudioService(Machine machine) : 
            base(machine)
        {
        }

        public void Output(int data) // machine thread
        {
            _buffer[_index] = (byte)data;
            _index = (_index + 1) % SampleSize;
            if (_index == 0)
            {
                _readEvent.Set();
                if (Machine.Settings.Cpu.IsThrottled)
                {
                    _writeEvent.WaitOne();
                }
            }
        }

        public void Reset()
        {
            Buffer.BlockCopy(SampleZero, 0, _buffer, 0, SampleSize);
        }

        public override void Stop() // main thread
        {
            _readEvent.Set(); // signal events; avoids deadlock
            _writeEvent.Set();
        }

        protected void Update(int bufferSize, Action<byte[], int> updateBuffer) // audio thread
        {
            if (Machine.State == MachineState.Running)
            {
                _readEvent.WaitOne();
            }
            if (updateBuffer != null)
            {
                updateBuffer(_buffer, bufferSize);
            }
            _writeEvent.Set();
        }

        public const int SampleRate = 44100; // hz
        public const int SampleChannels = 1;
        public const int SampleBits = 8;
        public const int SampleLatency = 40; // ms
        public const int SampleSize = (SampleRate * SampleLatency / 1000) * SampleChannels * (SampleBits / 8);

        private static readonly byte[] SampleZero = new byte[SampleSize];

        private byte[] _buffer = new byte[SampleSize];
        private int _index;

        private AutoResetEvent _readEvent = new AutoResetEvent(false);
        private AutoResetEvent _writeEvent = new AutoResetEvent(false);
    }
}
