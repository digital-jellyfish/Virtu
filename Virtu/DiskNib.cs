﻿using System;

namespace Jellyfish.Virtu
{
    public sealed class DiskNib : Disk525
    {
        public DiskNib(string name, byte[] data, bool isWriteProtected) : 
            base(name, data, isWriteProtected)
        {
        }

        public override void ReadTrack(int number, int fraction, byte[] buffer)
        {
            Buffer.BlockCopy(Data, (number / 2) * TrackSize, buffer, 0, TrackSize);
        }

        public override void WriteTrack(int number, int fraction, byte[] buffer)
        {
            Buffer.BlockCopy(buffer, 0, Data, (number / 2) * TrackSize, TrackSize);
        }
    }
}
