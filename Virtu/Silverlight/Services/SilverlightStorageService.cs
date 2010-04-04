using System;
using System.IO;
using System.IO.IsolatedStorage;

namespace Jellyfish.Virtu.Services
{
    public sealed class SilverlightStorageService : StorageService
    {
        public SilverlightStorageService(Machine machine) : 
            base(machine)
        {
        }

        public override void Load(string path, Action<Stream> reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (var stream = new IsolatedStorageFileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, store))
                    {
                        reader(stream);
                    }
                }
            }
            catch (FileNotFoundException)
            {
            }
            catch (IsolatedStorageException)
            {
            }
        }

        public override void Save(string path, Action<Stream> writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (var stream = new IsolatedStorageFileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, store))
                    {
                        writer(stream);
                    }
                }
            }
            catch (IsolatedStorageException)
            {
            }
        }
    }
}
