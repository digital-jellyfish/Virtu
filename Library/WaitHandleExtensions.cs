using System;
using System.Threading;

namespace Jellyfish.Library
{
    public static class WaitHandleExtensions
    {
#if XBOX
        public static bool WaitOne(this WaitHandle waitHandle, int millisecondsTimeout)
        {
            if (waitHandle == null)
            {
                throw new ArgumentNullException("waitHandle");
            }

            return waitHandle.WaitOne(millisecondsTimeout, false);
        }
#endif
    }
}
