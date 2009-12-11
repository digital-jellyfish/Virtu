using System;
using System.IO;
using Jellyfish.Library;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;

namespace Jellyfish.Virtu.Services
{
    public sealed class XnaStorageService : StorageService
    {
        public XnaStorageService(Machine machine, GameBase game) : 
            base(machine)
        {
            if (game == null)
            {
                throw new ArgumentNullException("game");
            }

            _game = game;
        }

        public override void Load(string path, Action<Stream> reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            try
            {
                using (StorageContainer storageContainer = _storageDevice.Value.OpenContainer(_game.Name))
                {
                    using (FileStream stream = new FileStream(Path.Combine(storageContainer.Path, path), FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        reader(stream);
                    }
                }
            }
            catch (FileNotFoundException)
            {
            }
        }

        public override void Save(string path, Action<Stream> writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            using (StorageContainer storageContainer = _storageDevice.Value.OpenContainer(_game.Name))
            {
                using (FileStream stream = new FileStream(Path.Combine(storageContainer.Path, path), FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    writer(stream);
                }
            }
        }

        private GameBase _game;
        private Lazy<StorageDevice> _storageDevice = new Lazy<StorageDevice>(() => Guide.EndShowStorageDeviceSelector(Guide.BeginShowStorageDeviceSelector(null, null)));
    }
}
