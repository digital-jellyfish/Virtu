using System;

namespace Jellyfish.Virtu.Services
{
    public sealed class WpfDebugService : DebugService
    {
        public WpfDebugService(Machine machine, MainWindow window) : 
            base(machine)
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }

            _window = window;
        }

        public override void WriteLine(string message)
        {
            message = string.Concat(DateTime.Now, " ", message, Environment.NewLine);

            if (_window.CheckAccess())
            {
                _window._debug.Text += message;
            }
            else
            {
                _window.Dispatcher.BeginInvoke(new Action(() => _window._debug.Text += message));
            }
        }

        private MainWindow _window;
    }
}
