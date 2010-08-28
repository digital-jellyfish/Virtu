using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Jellyfish.Library
{
    public static class GCHandleHelpers
    {
        [SecurityCritical]
        public static void Pin(object value, Action<IntPtr> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            var gcHandle = new GCHandle();
            try
            {
                gcHandle = GCHandle.Alloc(value, GCHandleType.Pinned);
                action(gcHandle.AddrOfPinnedObject());
            }
            finally
            {
                if (gcHandle.IsAllocated)
                {
                    gcHandle.Free();
                }
            }
        }
    }
}
