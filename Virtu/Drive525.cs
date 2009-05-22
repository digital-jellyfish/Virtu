using System;
using Jellyfish.Library;

namespace Jellyfish.Virtu
{
    public class Drive525
    {
        public Drive525()
        {
            DriveArmStepDelta[0] = new int[] { 0,  0,  1,  1,  0,  0,  1,  1, -1, -1,  0,  0, -1, -1,  0,  0 }; // phase 0
            DriveArmStepDelta[1] = new int[] { 0, -1,  0, -1,  1,  0,  1,  0,  0, -1,  0, -1,  1,  0,  1,  0 }; // phase 1
            DriveArmStepDelta[2] = new int[] { 0,  0, -1, -1,  0,  0, -1, -1,  1,  1,  0,  0,  1,  1,  0,  0 }; // phase 2
            DriveArmStepDelta[3] = new int[] { 0,  1,  0,  1, -1,  0, -1,  0,  0,  1,  0,  1, -1,  0, -1,  0 }; // phase 3
        }

        public void FlushTrack()
        {
            if (_trackChanged)
            {
                // TODO
                _trackChanged = false;
            }
        }

        public void InsertDisk(string fileName, int volume, bool isWriteProtected)
        {
            FlushTrack();

            // TODO handle null param/empty string for eject, or add Eject()

            byte[] fileData = FileHelpers.ReadAllBytes(fileName);
            _disk = Disk525.CreateDisk(fileName, fileData, volume, isWriteProtected);
            _trackLoaded = false;
        }

        public void ApplyPhaseChange(int phaseState)
        {
            // step the drive head according to stepper magnet changes
            int delta = DriveArmStepDelta[_trackNumber & 0x3][phaseState];
            if (delta != 0)
            {
                int newTrackNumber = MathHelpers.Clamp(_trackNumber + delta, 0, TrackNumberMax);
                if (newTrackNumber != _trackNumber)
                {
                    _trackNumber = newTrackNumber;
                    _trackOffset = 0;
                    _trackLoaded = false;
                }
            }
        }

        public int Read()
        {
            if (LoadTrack())
            {
                int data = _trackData[_trackOffset++];
                if (_trackOffset >= Disk525.TrackSize)
                {
                    _trackOffset = 0;
                }

                return data;
            }

            return _random.Next(0x01, 0xFF);
        }

        public void Write(int data)
        {
            if (LoadTrack())
            {
                _trackData[_trackOffset++] = (byte)data;
                if (_trackOffset >= Disk525.TrackSize)
                {
                    _trackOffset = 0;
                }
            }
        }

        private bool LoadTrack()
        {
            if (!_trackLoaded && (_disk != null))
            {
                _disk.ReadTrack(_trackNumber, 0, _trackData);
                _trackLoaded = true;
            }

            return _trackLoaded;
        }

        public bool IsWriteProtected { get { return _disk.IsWriteProtected; } }

        private const int TrackNumberMax = 0x44;

        private const int PhaseCount = 4;

        private readonly int[][] DriveArmStepDelta = new int[PhaseCount][];

        private Disk525 _disk;
        private bool _trackLoaded;
        private bool _trackChanged;
        private int _trackNumber;
        private int _trackOffset;
        private byte[] _trackData = new byte[Disk525.TrackSize];

        private Random _random = new Random();
    }
}
