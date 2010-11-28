using System;
using System.IO;

namespace Jellyfish.Virtu
{
    public abstract class MachineComponent
    {
        protected MachineComponent(Machine machine)
        {
            Machine = machine;
        }

        public virtual void Initialize()
        {
        }

        public virtual void Reset()
        {
        }

        public virtual void LoadState(BinaryReader reader, Version version)
        {
        }

        public virtual void Uninitialize()
        {
        }

        public virtual void SaveState(BinaryWriter writer)
        {
        }

        protected Machine Machine { get; private set; }
    }
}
