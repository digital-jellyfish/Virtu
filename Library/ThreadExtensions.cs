using System;
using System.Threading;

namespace Jellyfish.Library
{
    public static class ThreadExtensions
    {
        public static void IsAliveJoin(this Thread thread)
        {
            if (thread == null)
            {
                throw new ArgumentNullException("thread");
            }

#if !XBOX
            if (thread.IsAlive)
#endif
            {
                thread.Join();
            }
        }
    }
}
