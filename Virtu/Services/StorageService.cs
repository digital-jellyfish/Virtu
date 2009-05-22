using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Jellyfish.Virtu.Services
{
    public abstract class StorageService
    {
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public abstract string GetDiskFile();
        public abstract void Load(string path, Action<Stream> reader);
        public abstract void Save(string path, Action<Stream> writer);
    }
}
