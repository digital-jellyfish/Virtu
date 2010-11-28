using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

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

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "version")]
        public static Disk525 LoadState(BinaryReader reader, Version version)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            string name = reader.ReadString();
            bool isWriteProtected = reader.ReadBoolean();
            byte[] data = reader.ReadBytes(reader.ReadInt32());

            return CreateDisk(name, data, isWriteProtected);
        }

        public void SaveState(BinaryWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            writer.Write(Name);
            writer.Write(IsWriteProtected);
            writer.Write(Data.Length);
            writer.Write(Data);
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
