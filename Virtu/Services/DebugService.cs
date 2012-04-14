using System;
using System.Diagnostics;
using System.Globalization;

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

        public void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(CultureInfo.InvariantCulture, format, args));
        }

        protected virtual void OnWriteLine(string message)
        {
            Debug.WriteLine(message);
        }
    }
}
