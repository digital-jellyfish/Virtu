using System;
using System.Diagnostics.CodeAnalysis;

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

        public virtual void Start()
        {
        }

        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop")]
        public virtual void Stop()
        {
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        protected Machine Machine { get; private set; }
    }
}
