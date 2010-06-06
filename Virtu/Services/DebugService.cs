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

        public virtual void WriteLine(string message)
        {
            Debug.WriteLine(string.Concat(DateTime.Now, " ", message));
        }
    }
}
