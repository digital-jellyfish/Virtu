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
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, store))
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
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, store))
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
