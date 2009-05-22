using System;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace Jellyfish.Library
{
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
    public abstract class SafeAllocHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected SafeAllocHandle(bool ownsHandle) : 
            base(ownsHandle)
        {
        }
    }

    [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
    public sealed class SafeGlobalAllocHandle : SafeAllocHandle
    {
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public SafeGlobalAllocHandle() : 
            base(true)
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public SafeGlobalAllocHandle(IntPtr existingHandle, bool ownsHandle) : 
            base(ownsHandle)
        {
            SetHandle(existingHandle);
        }

        public static SafeGlobalAllocHandle Allocate(int size)
        {
            return Allocate(0x0, size);
        }

        public static SafeGlobalAllocHandle Allocate(byte[] value)
        {
            SafeGlobalAllocHandle alloc = Allocate(value.Length);
            Marshal.Copy(value, 0, alloc.DangerousGetHandle(), value.Length);

            return alloc;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
            return (NativeMethods.GlobalFree(handle) == IntPtr.Zero);
        }

        private static SafeGlobalAllocHandle Allocate(uint flags, int size)
        {
            SafeGlobalAllocHandle alloc = NativeMethods.GlobalAlloc(flags, (IntPtr)size);
            if (alloc.IsInvalid)
            {
                throw new Win32Exception();
            }

            return alloc;
        }

        [SuppressUnmanagedCodeSecurity]
        private static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern SafeGlobalAllocHandle GlobalAlloc(uint dwFlags, IntPtr sizetBytes);

            [DllImport("kernel32.dll", SetLastError = true)]
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            public static extern IntPtr GlobalFree(IntPtr hMem);
        }
    }

    [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
    public sealed class SafeLocalAllocHandle : SafeAllocHandle
    {
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public SafeLocalAllocHandle() : 
            base(true)
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public SafeLocalAllocHandle(IntPtr existingHandle, bool ownsHandle) : 
            base(ownsHandle)
        {
            SetHandle(existingHandle);
        }

        public static SafeLocalAllocHandle Allocate(int size)
        {
            return Allocate(0x0, size);
        }

        public static SafeLocalAllocHandle Allocate(byte[] value)
        {
            SafeLocalAllocHandle alloc = Allocate(value.Length);
            Marshal.Copy(value, 0, alloc.DangerousGetHandle(), value.Length);

            return alloc;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
            return (NativeMethods.LocalFree(handle) == IntPtr.Zero);
        }

        private static SafeLocalAllocHandle Allocate(uint flags, int size)
        {
            SafeLocalAllocHandle alloc = NativeMethods.LocalAlloc(flags, (IntPtr)size);
            if (alloc.IsInvalid)
            {
                throw new Win32Exception();
            }

            return alloc;
        }

        [SuppressUnmanagedCodeSecurity]
        private static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern SafeLocalAllocHandle LocalAlloc(uint dwFlags, IntPtr sizetBytes);

            [DllImport("kernel32.dll", SetLastError = true)]
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            public static extern IntPtr LocalFree(IntPtr hMem);
        }
    }
}
