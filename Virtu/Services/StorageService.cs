using System;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Security;
using Jellyfish.Library;
using Jellyfish.Virtu.Properties;

namespace Jellyfish.Virtu.Services
{
    public abstract class StorageService : MachineService
    {
        protected StorageService(Machine machine) : 
            base(machine)
        {
        }

        public abstract void Load(string fileName, Action<Stream> reader);

#if !WINDOWS
        [SecuritySafeCritical]
#endif
        public static void LoadFile(string fileName, Action<Stream> reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            try
            {
                using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    reader(stream);
                }
            }
            catch (FileNotFoundException)
            {
            }
        }

#if !WINDOWS
        [SecuritySafeCritical]
#endif
        public static void LoadFile(FileInfo fileInfo, Action<Stream> reader)
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException("fileInfo");
            }
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            try
            {
                using (var stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    reader(stream);
                }
            }
            catch (SecurityException)
            {
            }
        }

        public static void LoadResource(string resourceName, int resourceSize, Action<Stream> reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            try
            {
                using (var stream = GetResourceStream(resourceName, resourceSize))
                {
                    reader(stream);
                }
            }
            catch (FileNotFoundException)
            {
            }
        }

        public abstract void Save(string fileName, Action<Stream> writer);

#if !WINDOWS
        [SecuritySafeCritical]
#endif
        public static void SaveFile(string fileName, Action<Stream> writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            using (var stream = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                writer(stream);
            }
        }

#if !WINDOWS
        [SecuritySafeCritical]
#endif
        public static void SaveFile(FileInfo fileInfo, Action<Stream> writer)
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException("fileInfo");
            }
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            using (var stream = fileInfo.Open(FileMode.Create, FileAccess.Write, FileShare.None))
            {
                writer(stream);
            }
        }

        private static Stream GetResourceStream(string resourceName, int resourceSize)
        {
            var resourceStream = _resourceManager.Value.GetStream(resourceName, CultureInfo.CurrentUICulture);
            if (resourceStream == null)
            {
                throw new FileNotFoundException(string.Format(CultureInfo.CurrentUICulture, Strings.ResourceNotFound, resourceName));
            }
            if ((resourceSize > 0) && (resourceStream.Length != resourceSize))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, Strings.ResourceInvalid, resourceName));
            }

            return resourceStream;
        }

        private static Lazy<ResourceManager> _resourceManager = new Lazy<ResourceManager>(() => new ResourceManager("Jellyfish.Virtu.g", typeof(StorageService).Assembly) { IgnoreCase = true });
    }
}
