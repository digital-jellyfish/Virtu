using System;
using System.Linq;

namespace Jellyfish.Virtu.Services
{
    public class AudioService : MachineService
    {
        public AudioService(Machine machine) : 
            base(machine)
        {
        }

        public void ToggleOutput() // machine thread
        {
            lock (_lock)
            {
                long cycles = Machine.Cpu.Cycles;
                int toggleDelta = (int)(cycles - _toggleCycles);
                _toggleCycles = cycles;

                _deltaBuffer[_writeIndex] = toggleDelta;
                _writeIndex = (_writeIndex + 1) % DeltaBufferSize;
            }
        }

        protected void Update(int bufferSize, Action<byte[], int> updateBuffer) // audio thread
        {
            if (updateBuffer == null)
            {
                throw new ArgumentNullException("updateBuffer");
            }

            lock (_lock)
            {
                long cycles = Machine.Cpu.Cycles;
                int updateDelta = (int)(cycles - _updateCycles);
                _updateCycles = _toggleCycles = cycles; // reset audio frame

                if (updateDelta > 0)
                {
                    double bytesPerCycle = (double)bufferSize / (Machine.Settings.Cpu.IsThrottled ? CyclesPerSample : updateDelta);

                    while (_readIndex != _writeIndex)
                    {
                        int deltaSize = (int)(_deltaBuffer[_readIndex] * bytesPerCycle);
                        if (deltaSize > bufferSize)
                        {
                            _deltaBuffer[_readIndex] -= (int)((double)bufferSize / bytesPerCycle);
                            break;
                        }

                        updateBuffer(_isOutputHigh ? SampleHigh : SampleZero, deltaSize);
                        _isOutputHigh ^= true;

                        bufferSize -= deltaSize;
                        _readIndex = (_readIndex + 1) % DeltaBufferSize;
                    }

                    updateBuffer(_isOutputHigh ? SampleHigh : SampleZero, bufferSize);
                }
                else
                {
                    updateBuffer(SampleZero, bufferSize);
                }
            }
        }

        public const int SampleRate = 44100; // hz
        public const int SampleChannels = 1;
        public const int SampleBits = 8;
        public const int SampleLatency = 40; // ms
        public const int SampleSize = (int)(SampleRate * SampleLatency / 1000f) * SampleChannels * SampleBits / 8;

        private const int CyclesPerSecond = 1022730;
        private const int CyclesPerSample = (int)(CyclesPerSecond * SampleLatency / 1000f);

        private static readonly byte[] SampleHigh = Enumerable.Repeat((byte)0xFF, SampleSize).ToArray();
        private static readonly byte[] SampleZero = new byte[SampleSize];

        private const int DeltaBufferSize = 8192;

        private int[] _deltaBuffer = new int[DeltaBufferSize];
        private uint _readIndex;
        private uint _writeIndex;
        private bool _isOutputHigh;
        private long _toggleCycles;
        private long _updateCycles;
        private object _lock = new object();
    }
}
