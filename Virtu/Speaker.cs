using System;
using Jellyfish.Virtu.Services;

namespace Jellyfish.Virtu
{
    public sealed class Speaker : MachineComponent
    {
        public Speaker(Machine machine) :
            base(machine)
        {
            _flushOutputEvent = FlushOutputEvent; // cache delegates; avoids garbage
        }

        public override void Initialize()
        {
            _audioService = Machine.Services.GetService<AudioService>();

            Machine.Events.AddEvent(CyclesPerFlush * Machine.Settings.Cpu.Multiplier, _flushOutputEvent);
        }

        public void ToggleOutput()
        {
            UpdateCycles();
            _isHigh ^= true;
        }

        private void FlushOutputEvent()
        {
            UpdateCycles();
            _audioService.Output(_highCycles * 255 / _totalCycles); // quick and dirty decimation
            _highCycles = _totalCycles = 0;

            Machine.Events.AddEvent(CyclesPerFlush * Machine.Settings.Cpu.Multiplier, _flushOutputEvent);
        }

        private void UpdateCycles()
        {
            int delta = (int)(Machine.Cpu.Cycles - _lastCycles);
            if (_isHigh)
            {
                _highCycles += delta;
            }
            _totalCycles += delta;
            _lastCycles = Machine.Cpu.Cycles;
        }

        private const int CyclesPerFlush = 23;

        private Action _flushOutputEvent;

        private bool _isHigh;
        private int _highCycles;
        private int _totalCycles;
        private long _lastCycles;

        private AudioService _audioService;
    }
}
