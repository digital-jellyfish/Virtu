using System;

namespace Jellyfish.Virtu.Services
{
    public sealed class SilverlightDebugService : DebugService
    {
        public SilverlightDebugService(Machine machine, MainPage page) : 
            base(machine)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }

            _page = page;
        }

        public override void WriteLine(string message)
        {
            message = string.Concat(DateTime.Now, " ", message, Environment.NewLine);

            if (_page.CheckAccess())
            {
                _page._debug.Text += message;
            }
            else
            {
                _page.Dispatcher.BeginInvoke(() => _page._debug.Text += message);
            }
        }

        private MainPage _page;
    }
}
