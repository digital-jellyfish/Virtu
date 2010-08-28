using System;
using System.IO;
using Jellyfish.Library;
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
                using (var storageContainer = OpenContainer())
                {
                    using (var stream = storageContainer.OpenFile(path))
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

            using (var storageContainer = OpenContainer())
            {
                using (var stream = storageContainer.OpenFile(path))
                {
                    writer(stream);
                }
            }
        }

        private StorageContainer OpenContainer()
        {
            return _storageDevice.Value.EndOpenContainer(_storageDevice.Value.BeginOpenContainer(_game.Name, null, null));
        }

        private GameBase _game;
        private Lazy<StorageDevice> _storageDevice = new Lazy<StorageDevice>(() => StorageDevice.EndShowSelector(StorageDevice.BeginShowSelector(null, null)));
    }
}
