﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Jellyfish.Virtu.Services
{
    public class AsciiKeyEventArgs : EventArgs
    {
        private AsciiKeyEventArgs()
        {
        }

        public static AsciiKeyEventArgs Create(int asciiKey)
        {
            _instance.AsciiKey = asciiKey;

            return _instance;  // use singleton; avoids garbage
        }

        public int AsciiKey { get; private set; }

        private static readonly AsciiKeyEventArgs _instance = new AsciiKeyEventArgs();
    }

    public abstract class KeyboardService
    {
        public abstract bool IsKeyDown(int key);

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        protected void RaiseAsciiKeyDown(int asciiKey)
        {
            EventHandler<AsciiKeyEventArgs> handler = AsciiKeyDown;
            if (handler != null)
            {
                handler(this, AsciiKeyEventArgs.Create(asciiKey));
            }
        }

        public abstract void Update();

        public void WaitForKeyUp()
        {
            while (IsAnyKeyDown)
            {
                Thread.Sleep(100);
            }
        }

        public void WaitForResetKeyUp()
        {
            while (IsResetKeyDown)
            {
                Thread.Sleep(100);
            }
        }

        public event EventHandler<AsciiKeyEventArgs> AsciiKeyDown;

        public bool IsAnyKeyDown { get; protected set; }
        public bool IsOpenAppleKeyDown { get; protected set; }
        public bool IsCloseAppleKeyDown { get; protected set; }
        public bool IsResetKeyDown { get; protected set; }

        public bool IsCpuThrottleKeyDown { get; protected set; }
        public bool IsVideoFullScreenKeyDown { get; protected set; }
        public bool IsVideoMonochromeKeyDown { get; protected set; }
    }
}