using System;
using System.IO;

namespace Jellyfish.Virtu.Services
{
    public abstract class StorageService : MachineService
    {
        protected StorageService(Machine machine) : 
            base(machine)
        {
        }

        public abstract void Load(string path, Action<Stream> reader);
        public abstract void Save(string path, Action<Stream> writer);
    }
}
