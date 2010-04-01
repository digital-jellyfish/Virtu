using System;
using System.Threading;

namespace Jellyfish.Virtu.Services
{
    public sealed class AsciiKeyEventArgs : EventArgs
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

    public abstract class KeyboardService : MachineService
    {
        protected KeyboardService(Machine machine) : 
            base(machine)
        {
        }

        public abstract bool IsKeyDown(int key);

        public virtual void Update() // main thread
        {
            if (IsResetKeyDown)
            {
                if (!_resetKeyDown)
                {
                    _resetKeyDown = true; // entering reset; pause until key released
                    Machine.Pause();
                    Machine.Reset();
                }
            }
            else if (_resetKeyDown)
            {
                _resetKeyDown = false; // leaving reset
                Machine.Unpause();
            }
        }

        protected void OnAsciiKeyDown(int asciiKey)
        {
            EventHandler<AsciiKeyEventArgs> handler = AsciiKeyDown;
            if (handler != null)
            {
                handler(this, AsciiKeyEventArgs.Create(asciiKey));
            }
        }

        public event EventHandler<AsciiKeyEventArgs> AsciiKeyDown;

        public bool IsAnyKeyDown { get; protected set; }
        public bool IsOpenAppleKeyDown { get; protected set; }
        public bool IsCloseAppleKeyDown { get; protected set; }

        protected bool IsResetKeyDown { get; set; }

        private bool _resetKeyDown;
    }
}
