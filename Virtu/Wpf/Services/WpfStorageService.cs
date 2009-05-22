using System;
using System.Deployment.Application;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using Microsoft.Win32;

namespace Jellyfish.Virtu.Services
{
    public sealed class WpfStorageService : StorageService
    {
        public override string GetDiskFile()
        {
            string fileName = string.Empty;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Disk Files (*.nib)|*.nib|All Files (*.*)|*.*";
                bool? result = dialog.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    fileName = dialog.FileName;
                }
            }));

            return fileName;
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
