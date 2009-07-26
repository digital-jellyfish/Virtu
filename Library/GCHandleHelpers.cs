using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Jellyfish.Library
{
    public static class GCHandleHelpers
    {
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public static void Pin(object value, Action<IntPtr> action)
        {
            GCHandle gcHandle = new GCHandle();
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
