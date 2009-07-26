using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace Jellyfish.Library
{
    [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
    public static class MarshalHelpers
    {
        public static void FillMemory(IntPtr buffer, int bufferSize, byte value)
        {
            NativeMethods.FillMemory(buffer, (IntPtr)bufferSize, value);
        }

        public static void ZeroMemory(IntPtr buffer, int bufferSize)
        {
            NativeMethods.ZeroMemory(buffer, (IntPtr)bufferSize);
        }

        [SuppressUnmanagedCodeSecurity]
        private static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern void FillMemory(IntPtr destination, IntPtr length, byte fill);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern void ZeroMemory(IntPtr destination, IntPtr length);
        }
    }
}
