using System;
using System.Diagnostics.CodeAnalysis;

namespace Jellyfish.Virtu
{
    public abstract class Disk525
    {
        protected Disk525(string name, byte[] data, bool isWriteProtected)
        {
            Name = name;
            Data = data;
            IsWriteProtected = isWriteProtected;
        }

        public static Disk525 CreateDisk(string name, byte[] data, bool isWriteProtected)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (name.EndsWith(".nib", StringComparison.OrdinalIgnoreCase) && (data.Length == TrackCount * TrackSize))
            {
                return new DiskNib(name, data, isWriteProtected);
            }
            else if (name.EndsWith(".dsk", StringComparison.OrdinalIgnoreCase) && (data.Length == TrackCount * SectorCount * SectorSize))
            {
                return new DiskDsk(name, data, isWriteProtected);
            }

            return null;
        }

        public abstract void ReadTrack(int number, int fraction, byte[] buffer);
        public abstract void WriteTrack(int number, int fraction, byte[] buffer);

        public string Name { get; private set; }
        public bool IsWriteProtected { get; private set; }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        protected byte[] Data { get; private set; }

        public const int SectorCount = 16;
        public const int SectorSize = 0x100;
        public const int TrackCount = 35;
        public const int TrackSize = 0x1A00;
    }
}
