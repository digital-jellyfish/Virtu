using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Jellyfish.Virtu.Services
{
    public sealed class SilverlightStorageService : StorageService
    {
        public SilverlightStorageService(UserControl page)
        {
            _dispatcher = page.Dispatcher;
        }

        public override string GetDiskFile()
        {
            string fileName = string.Empty;

            // TODO
            //ManualResetEvent syncEvent = new ManualResetEvent(false);
            //DispatcherOperation operation = _dispatcher.BeginInvoke(() => 
            //{
            //    try
            //    {
            //        OpenFileDialog dialog = new OpenFileDialog(); // SL expects all dialogs to be user initiated, ie from within an event handler.
            //        dialog.Filter = "Disk Files (*.nib)|*.nib|All Files (*.*)|*.*";
            //        bool? result = dialog.ShowDialog();
            //        if (result.HasValue && result.Value)
            //        {
            //            fileName = dialog.File.FullName;
            //        }
            //    }
            //    finally
            //    {
            //        syncEvent.Set();
            //    }
            //});
            //syncEvent.WaitOne();

            return fileName;
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

        private Dispatcher _dispatcher;
    }
}
