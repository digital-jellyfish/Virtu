using System;
using Jellyfish.Library;

namespace Jellyfish.Virtu.Services
{
    public abstract class MachineService : IDisposable
    {
        protected MachineService(Machine machine)
        {
            Machine = machine;

            _debugService = new Lazy<DebugService>(() => Machine.Services.GetService<DebugService>());
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
        protected DebugService DebugService { get { return _debugService.Value; } }

        private Lazy<DebugService> _debugService;
    }
}
