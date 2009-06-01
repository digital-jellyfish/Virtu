using System.Diagnostics.CodeAnalysis;
using Jellyfish.Virtu.Services;
using Jellyfish.Virtu.Settings;

namespace Jellyfish.Virtu
{
    public sealed class DiskII : MachineComponent
    {
        public DiskII(Machine machine) :
            base(machine)
        {
        }

        public override void Initialize()
        {
            _storageService = Machine.Services.GetService<StorageService>();

            DiskIISettings settings = Machine.Settings.DiskII;
            if (settings.Disk1.Name.Length == 0)
            {
                settings.Disk1.Name = _storageService.GetDiskFile();
            }
            if (settings.Disk1.Name.Length > 0)
            {
                _drives[0].InsertDisk(settings.Disk1.Name, settings.Disk1.IsWriteProtected);
            }
            if (settings.Disk2.Name.Length > 0)
            {
                _drives[1].InsertDisk(settings.Disk2.Name, settings.Disk2.IsWriteProtected);
            }
        }

        public override void Reset()
        {
            _phaseStates = 0;
            SetMotorOn(false);
            SetDriveNumber(0);
            _loadMode = false;
            _writeMode = false;
        }

        public override void Uninitialize()
        {
            Flush();

            DiskIISettings settings = Machine.Settings.DiskII; // TODO remove; reset filename to prompt on next start
            settings.Disk1.Name = string.Empty;
            settings.Disk2.Name = string.Empty;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public int Read(int address)
        {
            switch (address & 0xF)
            {
                case 0x0: case 0x1: case 0x2: case 0x3: case 0x4: case 0x5: case 0x6: case 0x7:
                    SetPhase(address);
                    break;

                case 0x8:
                    SetMotorOn(false);
                    break;

                case 0x9:
                    SetMotorOn(true);
                    break;

                case 0xA:
                    SetDriveNumber(0);
                    break;

                case 0xB:
                    SetDriveNumber(1);
                    break;

                case 0xC:
                    _loadMode = false;
                    if (_motorOn && !_writeMode)
                    {
                        return _latch = _drives[_driveNumber].Read();
                    }
                    break;

                case 0xD:
                    _loadMode = true;
                    if (_motorOn && !_writeMode)
                    {
                        // write protect is forced if phase 1 is on [F9.7]
                        _latch &= 0x7F;
                        if (_drives[_driveNumber].IsWriteProtected || 
                            (_phaseStates & Phase1On) != 0)
                        {
                            _latch |= 0x80;
                        }
                    }
                    break;

                case 0xE:
                    _writeMode = false;
                    break;

                case 0xF:
                    _writeMode = true;
                    break;
            }

            if ((address & 1) == 0)
            {
                // only even addresses return the latch
                if (_motorOn)
                {
                    return _latch;
                }

                // simple hack to fool DOS SAMESLOT drive spin check (usually at $BD34)
                _driveSpin = !_driveSpin;
                return _driveSpin ? 0x7E : 0x7F;
            }

            return Machine.Video.ReadFloatingBus(); // [5-40]
        }

        public void Write(int address, int data)
        {
            switch (address & 0xF)
            {
                case 0x0: case 0x1: case 0x2: case 0x3: case 0x4: case 0x5: case 0x6: case 0x7:
                    SetPhase(address);
                    break;

                case 0x8:
                    SetMotorOn(false);
                    break;

                case 0x9:
                    SetMotorOn(true);
                    break;

                case 0xA: 
                    SetDriveNumber(0);
                    break;

                case 0xB:
                    SetDriveNumber(1);
                    break;

                case 0xC:
                    _loadMode = false;

                    // write protect is forced if phase 1 is on [F9.7]
                    if ((_phaseStates & Phase1On) != 0)
                    {
                        _drives[_driveNumber].Write(_latch);
                    }
                    break;

                case 0xD:
                    _loadMode = true;
                    break;

                case 0xE:
                    _writeMode = false;
                    break;

                case 0xF:
                    _writeMode = true;
                    break;
            }

            if (_motorOn && _writeMode)
            {
                if (_loadMode)
                {
                    // any address writes latch for sequencer LD; OE1/2 irrelevant ['323 datasheet]
                    _latch = data;
                }
            }
        }

        private void Flush()
        {
            _drives[_driveNumber].FlushTrack();
        }

        private void SetDriveNumber(int driveNumber)
        {
            if (_driveNumber != driveNumber)
            {
                Flush();
                _driveNumber = driveNumber;
            }
        }

        private void SetMotorOn(bool state)
        {
            if (_motorOn && !state)
            {
                Flush();
            }
            _motorOn = state;
        }

        private void SetPhase(int address)
        {
            int phase = (address >> 1) & 0x3;
            int state = address & 1;
            _phaseStates &= ~(1 << phase);
            _phaseStates |= (state << phase);

            if (_motorOn)
            {
                _drives[_driveNumber].ApplyPhaseChange(_phaseStates);
            }
        }

        private const int Phase0On = 1 << 0;
        private const int Phase1On = 1 << 1;
        private const int Phase2On = 1 << 2;
        private const int Phase3On = 1 << 3;

        private StorageService _storageService;

        private Drive525[] _drives = new Drive525[] { new Drive525(), new Drive525() };
        private int _latch;
        private int _phaseStates;
        private bool _motorOn;
        private int _driveNumber;
        private bool _loadMode;
        private bool _writeMode;
        private bool _driveSpin;
    }
}
