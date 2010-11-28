using System;
using Jellyfish.Library;

namespace Jellyfish.Virtu.Services
{
    public sealed class WpfDebugService : DebugService
    {
        public WpfDebugService(Machine machine, MainPage page) : 
            base(machine)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }

            _page = page;
        }

        protected override void OnWriteLine(string message)
        {
            _page.Dispatcher.Post(() => _page.WriteLine(message + Environment.NewLine));
        }

        private MainPage _page;
    }
}
