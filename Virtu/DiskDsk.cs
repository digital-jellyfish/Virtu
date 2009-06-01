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
            int track = number / 2;

            _trackBuffer = buffer;
            _trackOffset = 0;

            WriteNibble(0xFF, 48); // gap 0

            for (int sector = 0; sector < SectorCount; sector++)
            {
                WriteNibble(0xD5); // address prologue
                WriteNibble(0xAA);
                WriteNibble(0x96);

                WriteNibble44(Volume);
                WriteNibble44(track);
                WriteNibble44(sector);
                WriteNibble44(Volume ^ track ^ sector);

                WriteNibble(0xDE); // address epilogue
                WriteNibble(0xAA);
                WriteNibble(0xEB);
                WriteNibble(0xFF, 8);

                WriteNibble(0xD5); // data prologue
                WriteNibble(0xAA);
                WriteNibble(0xAD);

                WriteDataNibbles((track * SectorCount + DosOrderToLogicalSector[sector]) * SectorSize);

                WriteNibble(0xDE); // data epilogue
                WriteNibble(0xAA);
                WriteNibble(0xEB);
                WriteNibble(0xFF, 16);
            }
        }

        public override void WriteTrack(int number, int fraction, byte[] buffer)
        {
            // TODO
            throw new NotImplementedException();
        }

        private void WriteNibble(int data)
        {
            _trackBuffer[_trackOffset++] = (byte)data;
        }

        private void WriteNibble(int data, int count)
        {
            while (count-- > 0)
            {
                WriteNibble(data);
            }
        }

        private void WriteNibble44(int data)
        {
            WriteNibble((data >> 1) | 0xAA);
            WriteNibble(data | 0xAA);
        }

        private void WriteDataNibbles(int sectorOffset)
        {
            byte a, x, y;

            for (x = 0; x < SecondaryBufferLength; x++)
            {
                _secondaryBuffer[x] = 0; // zero secondary buffer
            }

            y = 2;
            do // fill buffers
            {
                x = 0;
                do
                {
                    a = Data[sectorOffset + --y];
                    _secondaryBuffer[x] = (byte)((_secondaryBuffer[x] << 2) | SwapBits[a & 0x03]); // b1,b0 -> secondary buffer
                    _primaryBuffer[y] = (byte)(a >> 2); // b7-b2 -> primary buffer
                }
                while (++x < SecondaryBufferLength);
            }
            while (y != 0);

            y = SecondaryBufferLength;
            do // write secondary buffer
            {
                WriteNibble(ByteToNibble[_secondaryBuffer[y] ^ _secondaryBuffer[y - 1]]);
            }
            while (--y != 0);

            a = _secondaryBuffer[0];
            do // write primary buffer
            {
                WriteNibble(ByteToNibble[a ^ _primaryBuffer[y]]);
                a = _primaryBuffer[y];
            }
            while (++y != 0);
            
            WriteNibble(ByteToNibble[a]); // data checksum
        }

        private byte[] _trackBuffer;
        private int _trackOffset;
        private byte[] _primaryBuffer = new byte[0x100];
        private const int SecondaryBufferLength = 0x56;
        private byte[] _secondaryBuffer = new byte[SecondaryBufferLength + 1];
        private const int Volume = 0xFE;

        private static readonly byte[] SwapBits = { 0, 2, 1, 3 };

        private static readonly int[] DosOrderToLogicalSector = new int[]
        {
            0x0, 0x7, 0xE, 0x6, 0xD, 0x5, 0xC, 0x4, 0xB, 0x3, 0xA, 0x2, 0x9, 0x1, 0x8, 0xF
        };

        private static readonly byte[] ByteToNibble = new byte[]
        {
            0x96, 0x97, 0x9A, 0x9B, 0x9D, 0x9E, 0x9F, 0xA6, 0xA7, 0xAB, 0xAC, 0xAD, 0xAE, 0xAF, 0xB2, 0xB3,
            0xB4, 0xB5, 0xB6, 0xB7, 0xB9, 0xBA, 0xBB, 0xBC, 0xBD, 0xBE, 0xBF, 0xCB, 0xCD, 0xCE, 0xCF, 0xD3,
            0xD6, 0xD7, 0xD9, 0xDA, 0xDB, 0xDC, 0xDD, 0xDE, 0xDF, 0xE5, 0xE6, 0xE7, 0xE9, 0xEA, 0xEB, 0xEC,
            0xED, 0xEE, 0xEF, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7, 0xF9, 0xFA, 0xFB, 0xFC, 0xFD, 0xFE, 0xFF
        };
    }
}
