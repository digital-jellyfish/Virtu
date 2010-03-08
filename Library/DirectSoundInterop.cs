using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace Jellyfish.Library
{
    public sealed partial class DirectSound
    {
        [Flags]
        private enum BufferCapabilities { PrimaryBuffer = 0x00000001, CtrlVolume = 0x00000080, CtrlPositionNotify = 0x00000100, StickyFocus = 0x00004000, GlobalFocus = 0x00008000 }

        [Flags]
        private enum BufferLock { None = 0x00000000, FromWriteCursor = 0x00000001, EntireBuffer = 0x00000002 }

        [Flags]
        private enum BufferPlay { Looping = 0x00000001 }

        [Flags]
        private enum BufferStatus { Playing = 0x00000001, BufferLost = 0x00000002, Looping = 0x00000004, Terminated = 0x00000020 }

        private enum CooperativeLevel { Normal = 1, Priority = 2 }

        [StructLayout(LayoutKind.Sequential)]
        private sealed class BufferDescription
        {
            public BufferDescription(BufferCapabilities capabilities, int size, IntPtr format)
            {
                dwSize = Marshal.SizeOf(typeof(BufferDescription));
                dwFlags = capabilities;
                dwBufferBytes = size;
                lpwfxFormat = format;
            }

            public int dwSize;
            public BufferCapabilities dwFlags;
            public int dwBufferBytes;
            public int dwReserved;
            public IntPtr lpwfxFormat;
            public Guid guid3DAlgorithm;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BufferPositionNotify
        {
            [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Runtime.InteropServices.SafeHandle.DangerousGetHandle")]
            public BufferPositionNotify(int offset, EventWaitHandle notifyEvent)
            {
                dwOffset = offset;
                hEventNotify = notifyEvent.SafeWaitHandle.DangerousGetHandle();
            }

            public int dwOffset;
            public IntPtr hEventNotify;
        }

        [ComImport, Guid("279AFA83-4981-11CE-A521-0020AF0BE560"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IDirectSound
        {
            void CreateSoundBuffer(BufferDescription pcDSBufferDesc, [MarshalAs(UnmanagedType.Interface)] out IDirectSoundBuffer pDSBuffer, IntPtr pUnkOuter);
            void GetCaps(IntPtr pDSCaps);
            void DuplicateSoundBuffer([MarshalAs(UnmanagedType.Interface)] IDirectSoundBuffer pDSBufferOriginal, [MarshalAs(UnmanagedType.Interface)] out IDirectSoundBuffer pDSBufferDuplicate);
            void SetCooperativeLevel(IntPtr hwnd, CooperativeLevel dwLevel);
            void Compact();
            void GetSpeakerConfig(out int dwSpeakerConfig);
            void SetSpeakerConfig(int dwSpeakerConfig);
            void Initialize([MarshalAs(UnmanagedType.LPStruct)] Guid pcGuidDevice);
        }

        [ComImport, Guid("279AFA85-4981-11CE-A521-0020AF0BE560"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IDirectSoundBuffer
        {
            void GetCaps(IntPtr pDSBufferCaps);
            void GetCurrentPosition(out int dwCurrentPlayCursor, out int dwCurrentWriteCursor);
            void GetFormat(IntPtr pwfxFormat, int dwSizeAllocated, out int dwSizeWritten);
            void GetVolume(out int lVolume);
            void GetPan(out int lPan);
            void GetFrequency(out int dwFrequency);
            void GetStatus(out BufferStatus dwStatus);
            void Initialize([MarshalAs(UnmanagedType.Interface)] IDirectSound pDirectSound, BufferDescription pcDSBufferDesc);
            void Lock(int dwOffset, int dwBytes, out IntPtr pvAudioPtr1, out int dwAudioBytes1, out IntPtr pvAudioPtr2, out int dwAudioBytes2, BufferLock dwFlags);
            void Play(int dwReserved1, int dwPriority, BufferPlay dwFlags);
            void SetCurrentPosition(int dwNewPosition);
            void SetFormat(WaveFormat pcfxFormat);
            void SetVolume(int lVolume);
            void SetPan(int lPan);
            void SetFrequency(int dwFrequency);
            void Stop();
            void Unlock(IntPtr pvAudioPtr1, int dwAudioBytes1, IntPtr pvAudioPtr2, int dwAudioBytes2);
            void Restore();
        }

        [ComImport, Guid("B0210783-89CD-11D0-AF08-00A0C925CD16"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IDirectSoundNotify
        {
            void SetNotificationPositions(int dwPositionNotifies, [MarshalAs(UnmanagedType.LPArray)] BufferPositionNotify[] pcPositionNotifies);
        }

        [SuppressUnmanagedCodeSecurity]
        private static class NativeMethods
        {
            [DllImport("dsound.dll")]
            public static extern int DirectSoundCreate(IntPtr pcGuidDevice, [MarshalAs(UnmanagedType.Interface)] out IDirectSound pDS, IntPtr pUnkOuter);
        }
    }
}
