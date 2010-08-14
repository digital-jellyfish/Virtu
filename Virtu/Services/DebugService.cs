using System;
using System.Diagnostics;

namespace Jellyfish.Virtu.Services
{
    public class DebugService : MachineService
    {
        public DebugService(Machine machine) : 
            base(machine)
        {
        }

        public void WriteLine(string message)
        {
            OnWriteLine(string.Concat(DateTime.Now.TimeOfDay, ' ', message));
        }

        protected virtual void OnWriteLine(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
