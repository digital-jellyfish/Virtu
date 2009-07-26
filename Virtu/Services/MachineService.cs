using System;

namespace Jellyfish.Virtu.Services
{
    public abstract class MachineService : IDisposable
    {
        protected MachineService(Machine machine)
        {
            Machine = machine;
        }

        ~MachineService()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        protected Machine Machine { get; private set; }
    }
}
