using System;
using System.Windows.Threading;

namespace Jellyfish.Library
{
    public static class DispatcherExtensions
    {
        public static void CheckBeginInvoke(this Dispatcher dispatcher, Action action)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            if (dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                dispatcher.BeginInvoke(action);
            }
        }

#if WINDOWS
        public static void CheckInvoke(this Dispatcher dispatcher, Action action)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            if (dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                dispatcher.Invoke(action);
            }
        }
#endif
    }
}
