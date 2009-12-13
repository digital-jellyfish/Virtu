using System;
using System.Diagnostics.CodeAnalysis;
using Jellyfish.Virtu.Services;
using Jellyfish.Virtu.Settings;

namespace Jellyfish.Virtu
{
    public sealed class Keyboard : MachineComponent
    {
        public Keyboard(Machine machine) :
            base(machine)
        {
        }

        public override void Initialize()
        {
            _keyboardService = Machine.Services.GetService<KeyboardService>();
            _gamePortService = Machine.Services.GetService<GamePortService>();

            _keyboardService.AsciiKeyDown += (sender, e) => Latch = e.AsciiKey;
            Machine.Video.VSync += OnVideoVSync;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public int ReadLatch()
        {
            if (Strobe)
            {
                return Latch;
            }

            KeyboardSettings settings = Machine.Settings.Keyboard;

            if (settings.UseGamePort)
            {
                if ((settings.Key.Joystick0.UpLeft > 0) && _gamePortService.Joystick0.IsUp && _gamePortService.Joystick0.IsLeft)
                {
                    Latch = settings.Key.Joystick0.UpLeft;
                }
                else if ((settings.Key.Joystick0.UpRight > 0) && _gamePortService.Joystick0.IsUp && _gamePortService.Joystick0.IsRight)
                {
                    Latch = settings.Key.Joystick0.UpRight;
                }
                else if ((settings.Key.Joystick0.DownLeft > 0) && _gamePortService.Joystick0.IsDown && _gamePortService.Joystick0.IsLeft)
                {
                    Latch = settings.Key.Joystick0.DownLeft;
                }
                else if ((settings.Key.Joystick0.DownRight > 0) && _gamePortService.Joystick0.IsDown && _gamePortService.Joystick0.IsRight)
                {
                    Latch = settings.Key.Joystick0.DownRight;
                }
                else if ((settings.Key.Joystick0.Up > 0) && _gamePortService.Joystick0.IsUp)
                {
                    Latch = settings.Key.Joystick0.Up;
                }
                else if ((settings.Key.Joystick0.Left > 0) && _gamePortService.Joystick0.IsLeft)
                {
                    Latch = settings.Key.Joystick0.Left;
                }
                else if ((settings.Key.Joystick0.Right > 0) && _gamePortService.Joystick0.IsRight)
                {
                    Latch = settings.Key.Joystick0.Right;
                }
                else if ((settings.Key.Joystick0.Down > 0) && _gamePortService.Joystick0.IsDown)
                {
                    Latch = settings.Key.Joystick0.Down;
                }

                if ((settings.Key.Joystick1.UpLeft > 0) && _gamePortService.Joystick1.IsUp && _gamePortService.Joystick1.IsLeft) // override
                {
                    Latch = settings.Key.Joystick1.UpLeft;
                }
                else if ((settings.Key.Joystick1.UpRight > 0) && _gamePortService.Joystick1.IsUp && _gamePortService.Joystick1.IsRight)
                {
                    Latch = settings.Key.Joystick1.UpRight;
                }
                else if ((settings.Key.Joystick1.DownLeft > 0) && _gamePortService.Joystick1.IsDown && _gamePortService.Joystick1.IsLeft)
                {
                    Latch = settings.Key.Joystick1.DownLeft;
                }
                else if ((settings.Key.Joystick1.DownRight > 0) && _gamePortService.Joystick1.IsDown && _gamePortService.Joystick1.IsRight)
                {
                    Latch = settings.Key.Joystick1.DownRight;
                }
                else if ((settings.Key.Joystick1.Up > 0) && _gamePortService.Joystick1.IsUp)
                {
                    Latch = settings.Key.Joystick1.Up;
                }
                else if ((settings.Key.Joystick1.Left > 0) && _gamePortService.Joystick1.IsLeft)
                {
                    Latch = settings.Key.Joystick1.Left;
                }
                else if ((settings.Key.Joystick1.Right > 0) && _gamePortService.Joystick1.IsRight)
                {
                    Latch = settings.Key.Joystick1.Right;
                }
                else if ((settings.Key.Joystick1.Down > 0) && _gamePortService.Joystick1.IsDown)
                {
                    Latch = settings.Key.Joystick1.Down;
                }

                if ((settings.Key.Button0 > 0) && _gamePortService.IsButton0Down) // override
                {
                    Latch = settings.Key.Button0;
                }
                else if ((settings.Key.Button1 > 0) && _gamePortService.IsButton1Down)
                {
                    Latch = settings.Key.Button1;
                }
                else if ((settings.Key.Button2 > 0) && _gamePortService.IsButton2Down)
                {
                    Latch = settings.Key.Button2;
                }
            }

            return Latch;
        }

        public void ResetStrobe()
        {
            Strobe = false;
        }

        private void OnVideoVSync(object sender, EventArgs e)
        {
            if (_keyboardService.IsResetKeyDown)
            {
                Machine.Reset();
                _keyboardService.WaitForResetKeyUp();
            }
            else if (_keyboardService.IsCpuThrottleKeyDown)
            {
                Machine.Cpu.ToggleThrottle();
                _keyboardService.WaitForKeyUp();
            }
            else if (_keyboardService.IsVideoFullScreenKeyDown)
            {
                Machine.Video.ToggleFullScreen();
                _keyboardService.WaitForKeyUp();
            }
            else if (_keyboardService.IsVideoMonochromeKeyDown)
            {
                Machine.Video.ToggleMonochrome();
                _keyboardService.WaitForKeyUp();
            }
        }

        public bool IsAnyKeyDown { get { return _keyboardService.IsAnyKeyDown; } }
        public bool Strobe { get; private set; }

        private int Latch { get { return _latch; } set { _latch = value; Strobe = true; } }

        private KeyboardService _keyboardService;
        private GamePortService _gamePortService;

        private int _latch;
    }
}
