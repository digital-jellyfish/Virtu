using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using Microsoft.Win32.SafeHandles;

namespace Jellyfish.Library
{
    [SecurityCritical]
    public sealed class SafeFileHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public SafeFileHandle() : 
            base(ownsHandle: true)
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public SafeFileHandle(IntPtr existingHandle, bool ownsHandle) : 
            base(ownsHandle)
        {
            SetHandle(existingHandle);
        }

        [SecurityCritical]
        public static SafeFileHandle CreateFile(string fileName, FileAccess fileAccess, FileShare fileShare, FileMode fileMode, GeneralSecurity fileSecurity)
        {
            if (fileMode == FileMode.Append)
            {
                throw new NotImplementedException();
            }

            bool inheritable = ((fileShare & FileShare.Inheritable) != 0);
            fileShare &= ~FileShare.Inheritable;

            return CreateFile(fileName, (uint)fileAccess, (uint)fileShare, (uint)fileMode, 0x0, fileSecurity, inheritable);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SecurityCritical]
        protected override bool ReleaseHandle()
        {
            return NativeMethods.CloseHandle(handle);
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public GeneralSecurity GetAccessControl()
        {
            return new GeneralSecurity(false, ResourceType.FileObject, this);
        }

        public void SetAccessControl(GeneralSecurity fileSecurity)
        {
            if (fileSecurity == null)
            {
                throw new ArgumentNullException("fileSecurity");
            }

            fileSecurity.Persist(this);
        }

        [SecurityCritical]
        private static SafeFileHandle CreateFile(string fileName, uint fileAccess, uint fileShare, uint fileMode, uint fileOptions, GeneralSecurity fileSecurity, 
            bool inheritable = false)
        {
            var file = new SafeFileHandle();

            GeneralSecurity.GetSecurityAttributes(fileSecurity, securityAttributes => 
            {
                file = NativeMethods.CreateFile(fileName, fileAccess, fileShare, securityAttributes, fileMode, fileOptions, IntPtr.Zero);
                if (file.IsInvalid)
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
            }, inheritable);

            return file;
        }

        [SecurityCritical]
        [SuppressUnmanagedCodeSecurity]
        private static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CloseHandle(IntPtr handle);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, SecurityAttributes lpSecurityAttributes, 
                uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);
        }
    }
}
