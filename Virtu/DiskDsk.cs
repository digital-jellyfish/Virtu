using System;

namespace Jellyfish.Virtu
{
    public sealed class DiskDsk : Disk525
    {
        public DiskDsk(string name, byte[] data, bool isWriteProtected) : 
            base(name, data, isWriteProtected)
        {
        }

        public override void ReadTrack(int number, int fraction, byte[] buffer)
        {
            // TODO
            throw new NotImplementedException();
        }

        public override void WriteTrack(int number, int fraction, byte[] buffer)
        {
            // TODO
            throw new NotImplementedException();
        }

        private const int Volume = 0xFE;
    }
}
