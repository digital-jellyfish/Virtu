using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Jellyfish.Virtu.Services
{
    public sealed class XnaKeyboardService : KeyboardService
    {
        public XnaKeyboardService(Machine machine) : 
            base(machine)
        {
        }

        public override bool IsKeyDown(int key)
        {
            return IsKeyDown((Keys)key);
        }

        public override void Update() // main thread
        {
            _lastState = _state;
            _state = Microsoft.Xna.Framework.Input.Keyboard.GetState();

            var gamePadState = GamePad.GetState(PlayerIndex.One);
            bool gamePadControl = (gamePadState.Buttons.LeftStick == ButtonState.Pressed);

            if (_state != _lastState)
            {
                IsAnyKeyDown = false;
                foreach (Keys key in KeyValues) // xna doesn't support buffered input; loses input order and could lose keys between updates
                {
                    if (_state.IsKeyDown(key))
                    {
                        IsAnyKeyDown = true;
                        if (!_lastState.IsKeyDown(key))
                        {
                            _lastKey = key;
                            _lastTime = DateTime.UtcNow.Ticks;
                            _repeatTime = RepeatDelay;
                            OnKeyDown(key, gamePadControl);
                        }
                    }
                    else
                    {
                        if (key == _lastKey)
                        {
                            _lastKey = Keys.None;
                        }
                        if (_lastState.IsKeyDown(key))
                        {
                            OnKeyUp(key, gamePadControl);
                        }
                    }
                }
            }

            if (_lastKey != Keys.None) // repeat last key
            {
                long time = DateTime.UtcNow.Ticks;
                if (time - _lastTime >= _repeatTime)
                {
                    _lastTime = time;
                    _repeatTime = RepeatSpeed;
                    OnKeyDown(_lastKey, gamePadControl);
                }
            }

            bool control = IsKeyDown(Keys.LeftControl) || IsKeyDown(Keys.RightControl);

            IsOpenAppleKeyDown = IsKeyDown(Keys.LeftAlt) || IsKeyDown(Keys.NumPad0) || (gamePadState.Buttons.LeftShoulder == ButtonState.Pressed);
            IsCloseAppleKeyDown = IsKeyDown(Keys.RightAlt) || IsKeyDown(Keys.Decimal) || (gamePadState.Buttons.RightShoulder == ButtonState.Pressed);
            IsResetKeyDown = (control && IsKeyDown(Keys.Back)) || (gamePadControl && (gamePadState.Buttons.Start == ButtonState.Pressed));

            base.Update();
        }

        private bool IsKeyDown(Keys key)
        {
            return _state.IsKeyDown(key);
        }

        private void OnKeyDown(Keys key, bool gamePadControl)
        {
            int asciiKey = GetAsciiKey(key, gamePadControl);
            if (asciiKey >= 0)
            {
                Machine.Keyboard.Latch = asciiKey;
            }
        }

        private void OnKeyUp(Keys key, bool gamePadControl)
        {
            bool control = IsKeyDown(Keys.LeftControl) || IsKeyDown(Keys.RightControl);

            if (key == Keys.CapsLock)
            {
                _capsLock ^= true;
            }
            else if ((control && (key == Keys.Divide)) || (gamePadControl && (key == Keys.D8)))
            {
                Machine.Cpu.ToggleThrottle();
            }
            else if ((control && (key == Keys.Multiply)) || (gamePadControl && (key == Keys.D9)))
            {
                Machine.Video.ToggleMonochrome();
            }
#if WINDOWS
            else if ((control && (key == Keys.Subtract)) || (gamePadControl && (key == Keys.D0)))
            {
                Machine.Video.ToggleFullScreen();
            }
#endif
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
        private int GetAsciiKey(Keys key, bool gamePadControl)
        {
            bool control = IsKeyDown(Keys.LeftControl) || IsKeyDown(Keys.RightControl) || gamePadControl;
            bool shift = IsKeyDown(Keys.LeftShift) || IsKeyDown(Keys.RightShift);
            bool capsLock = shift ^ _capsLock;
            bool green = IsKeyDown(Keys.ChatPadGreen);
            bool orange = IsKeyDown(Keys.ChatPadOrange);

            switch (key)
            {
            case Keys.Left:
                return 0x08;

            case Keys.Tab:
                return 0x09;

            case Keys.Down:
                return 0x0A;

            case Keys.Up:
                return 0x0B;

            case Keys.Enter:
                return 0x0D;

            case Keys.Right:
                return 0x15;

            case Keys.Escape:
                return 0x1B;

            case Keys.Back:
                return control ? -1 : 0x7F;

            case Keys.Space:
                return ' ';

            case Keys.D1:
                return shift ? '!' : '1';

            case Keys.D2:
                return control ? 0x00 : shift ? '@' : '2';

            case Keys.D3:
                return shift ? '#' : '3';

            case Keys.D4:
                return shift ? '$' : '4';

            case Keys.D5:
                return shift ? '%' : '5';

            case Keys.D6:
                return control ? 0x1E : shift ? '^' : '6';

            case Keys.D7:
                return shift ? '&' : '7';

            case Keys.D8:
                return gamePadControl ? -1 : shift ? '*' : '8';

            case Keys.D9:
                return gamePadControl ? -1 : shift ? '(' : '9';

            case Keys.D0:
                return gamePadControl ? -1 : shift ? ')' : '0';

            case Keys.A:
                return control ? 0x01 : green ? '~' : capsLock ? 'A' : 'a';

            case Keys.B:
                return control ? 0x02 : green ? '|' : orange ? '+' : capsLock ? 'B' : 'b';

            case Keys.C:
                return control ? 0x03 : capsLock ? 'C' : 'c';

            case Keys.D:
                return control ? 0x04 : green ? '{' : capsLock ? 'D' : 'd';

            case Keys.E:
                return control ? 0x05 : capsLock ? 'E' : 'e';

            case Keys.F:
                return control ? 0x06 : green ? '}' : capsLock ? 'F' : 'f';

            case Keys.G:
                return control ? 0x07 : capsLock ? 'G' : 'g';

            case Keys.H:
                return control ? 0x08 : green ? '/' : orange ? '\\' : capsLock ? 'H' : 'h';

            case Keys.I:
                return control ? 0x09 : green ? '*' : capsLock ? 'I' : 'i';

            case Keys.J:
                return control ? 0x0A : green ? '\'' : orange ? '"' : capsLock ? 'J' : 'j';

            case Keys.K:
                return control ? 0x0B : green ? '[' : capsLock ? 'K' : 'k';

            case Keys.L:
                return control ? 0x0C : green ? ']' : capsLock ? 'L' : 'l';

            case Keys.M:
                return control ? 0x0D : green ? '>' : capsLock ? 'M' : 'm';

            case Keys.N:
                return control ? 0x0E : green ? '<' : capsLock ? 'N' : 'n';

            case Keys.O:
                return control ? 0x0F : green ? '(' : capsLock ? 'O' : 'o';

            case Keys.P:
                return control ? 0x10 : green ? ')' : orange ? '=' : capsLock ? 'P' : 'p';

            case Keys.Q:
                return control ? 0x11 : green ? '!' : capsLock ? 'Q' : 'q';

            case Keys.R:
                return control ? 0x12 : green ? '#' : orange ? '$' : capsLock ? 'R' : 'r';

            case Keys.S:
                return control ? 0x13 : capsLock ? 'S' : 's';

            case Keys.T:
                return control ? 0x14 : green ? '%' : capsLock ? 'T' : 't';

            case Keys.U:
                return control ? 0x15 : green ? '&' : capsLock ? 'U' : 'u';

            case Keys.V:
                return control ? 0x16 : green ? '-' : orange ? '_' : capsLock ? 'V' : 'v';

            case Keys.W:
                return control ? 0x17 : green ? '@' : capsLock ? 'W' : 'w';

            case Keys.X:
                return control ? 0x18 : capsLock ? 'X' : 'x';

            case Keys.Y:
                return control ? 0x19 : green ? '^' : capsLock ? 'Y' : 'y';

            case Keys.Z:
                return control ? 0x1A : green ? '`' : capsLock ? 'Z' : 'z';

            case Keys.OemSemicolon:
                return shift ? ':' : ';';

            case Keys.OemQuestion:
                return shift ? '?' : '/';

            case Keys.OemTilde:
                return shift ? '~' : '`';

            case Keys.OemOpenBrackets:
                return shift ? '{' : '[';

            case Keys.OemBackslash:
            case Keys.OemPipe:
                return control ? 0x1C : shift ? '|' : '\\';

            case Keys.OemCloseBrackets:
                return control ? 0x1D : shift ? '}' : ']';

            case Keys.OemQuotes:
                return shift ? '"' : '\'';

            case Keys.OemMinus:
                return control ? 0x1F : shift ? '_' : '-';

            case Keys.OemPlus:
                return shift ? '+' : '=';

            case Keys.OemComma:
                return shift ? '<' : green ? ':' : orange ? ';' : ',';

            case Keys.OemPeriod:
                return shift ? '>' : green ? '?' : '.';
            }

            return -1;
        }

        private static readonly Keys[] KeyValues = 
#if WINDOWS_PHONE || XBOX
            (from key in 
                (from field in typeof(Keys).GetFields() // missing Enum.GetValues; use reflection
                where field.IsLiteral
                select (Keys)field.GetValue(typeof(Keys)))
            where (key != Keys.None) // filter Keys.None
            select key).ToArray();
#else
            (from key in (Keys[])Enum.GetValues(typeof(Keys))
             where (key != Keys.None) // filter Keys.None
             select key).ToArray();
#endif
        private static readonly long RepeatDelay = TimeSpan.FromMilliseconds(500).Ticks;
        private static readonly long RepeatSpeed = TimeSpan.FromMilliseconds(32).Ticks;

        private KeyboardState _state;
        private KeyboardState _lastState;
        private bool _capsLock;
        private Keys _lastKey;
        private long _lastTime;
        private long _repeatTime;
    }
}
