using System;
using System.Windows.Threading;

namespace Jellyfish.Library
{
    public static class DispatcherExtensions
    {
        public static void Post(this Dispatcher dispatcher, Action action)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            new DispatcherSynchronizationContext(dispatcher).Post(state => action(), null);
        }

        public static void Send(this Dispatcher dispatcher, Action action)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            new DispatcherSynchronizationContext(dispatcher).Send(state => action(), null);
        }
    }
}
