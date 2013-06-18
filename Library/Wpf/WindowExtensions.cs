using System;
using System.Windows;
using System.Windows.Interop;

namespace Jellyfish.Library
{
    public static class WindowExtensions
    {
        public static IntPtr GetHandle(this Window window)
        {
            return new WindowInteropHelper(window).Handle;
        }
    }
}
