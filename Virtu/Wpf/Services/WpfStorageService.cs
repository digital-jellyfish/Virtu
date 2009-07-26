using System;
using System.Deployment.Application;
using System.IO;
using System.IO.IsolatedStorage;

namespace Jellyfish.Virtu.Services
{
    public sealed class WpfStorageService : StorageService
    {
        public WpfStorageService(Machine machine) : 
            base(machine)
        {
        }

        public override void Load(string path, Action<Stream> reader)
        {
            try
            {
                using (IsolatedStorageFile store = GetStore())
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
                using (IsolatedStorageFile store = GetStore())
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

        private static IsolatedStorageFile GetStore()
        {
            return ApplicationDeployment.IsNetworkDeployed ? // clickonce
                IsolatedStorageFile.GetUserStoreForApplication() : IsolatedStorageFile.GetUserStoreForAssembly();
        }
    }
}
