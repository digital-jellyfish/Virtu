using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Jellyfish.Virtu.Services
{
    public sealed class SilverlightKeyboardService : KeyboardService
    {
        public SilverlightKeyboardService(Machine machine, UserControl page) : 
            base(machine)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }

            page.KeyDown += OnPageKeyDown;
            page.KeyUp += OnPageKeyUp;
            page.LostFocus += OnPageLostFocus;
        }

        public override bool IsKeyDown(int key)
        {
            return IsKeyDown((Key)key);
        }

        public override void Update() // main thread
        {
            if (_updateAnyKeyDown) // SL is missing access to keyboard state; could lose track of keyboard state after Alt+Tab
            {
                _updateAnyKeyDown = false;
                IsAnyKeyDown = false;
                foreach (Key key in KeyValues)
                {
                    if (IsKeyDown(key))
                    {
                        IsAnyKeyDown = true;
                        break;
                    }
                }
            }

            var modifiers = System.Windows.Input.Keyboard.Modifiers;
            IsControlKeyDown = ((modifiers & ModifierKeys.Control) != 0);
            IsShiftKeyDown = ((modifiers & ModifierKeys.Shift) != 0);

            IsOpenAppleKeyDown = ((modifiers & ModifierKeys.Alt) != 0) || IsKeyDown(Key.NumPad0);
            IsCloseAppleKeyDown = ((modifiers & ModifierKeys.Windows) != 0) || IsKeyDown(Key.Decimal);
            IsResetKeyDown = IsControlKeyDown && IsKeyDown(Key.Back);

            base.Update();
        }

        private bool IsKeyDown(Key key)
        {
            return _states[(int)key];
        }

        private void OnPageKeyDown(object sender, KeyEventArgs e)
        {
            //DebugService.WriteLine(string.Concat("OnPageKeyDn: Key=", e.Key, " PlatformKeyCode=", e.PlatformKeyCode));

            _states[(int)e.Key] = true;
            _updateAnyKeyDown = false;
            IsAnyKeyDown = true;

            int asciiKey = GetAsciiKey(e.Key, e.PlatformKeyCode);
            if (asciiKey >= 0)
            {
                Machine.Keyboard.Latch = asciiKey;
                e.Handled = true;
            }

            Update();
        }

        private void OnPageKeyUp(object sender, KeyEventArgs e)
        {
            //DebugService.WriteLine(string.Concat("OnPageKeyUp: Key=", e.Key, " PlatformKeyCode=", e.PlatformKeyCode));

            _states[(int)e.Key] = false;
            _updateAnyKeyDown = true;

            var modifiers = System.Windows.Input.Keyboard.Modifiers;
            bool control = ((modifiers & ModifierKeys.Control) != 0);

            if (e.Key == Key.CapsLock)
            {
                _capsLock ^= true; // SL is missing caps lock support; try to track manually
            }
            else if (control && (e.Key == Key.Divide))
            {
                Machine.Cpu.ToggleThrottle();
            }
            else if (control && (e.Key == Key.Multiply))
            {
                Machine.Video.ToggleMonochrome();
            }
            else if (control && (e.Key == Key.Subtract))
            {
                Machine.Video.ToggleFullScreen();
            }

            Update();
        }

        private void OnPageLostFocus(object sender, RoutedEventArgs e) // reset keyboard state on lost focus; can't access keyboard state on got focus
        {
            IsAnyKeyDown = false;
            foreach (Key key in KeyValues)
            {
                _states[(int)key] = false;
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
        private int GetAsciiKey(Key key, int platformKeyCode)
        {
            var modifiers = System.Windows.Input.Keyboard.Modifiers;
            bool control = ((modifiers & ModifierKeys.Control) != 0);
            bool shift = ((modifiers & ModifierKeys.Shift) != 0);
            bool capsLock = shift ^ _capsLock;

            switch (key)
            {
            case Key.Left:
                return 0x08;

            case Key.Tab:
                return 0x09;

            case Key.Down:
                return 0x0A;

            case Key.Up:
                return 0x0B;

            case Key.Enter:
                return 0x0D;

            case Key.Right:
                return 0x15;

            case Key.Escape:
                return 0x1B;

            case Key.Back:
                return control ? -1 : 0x7F;

            case Key.Space:
                return ' ';

            case Key.D1:
                return shift ? '!' : '1';

            case Key.D2:
                return control ? 0x00 : shift ? '@' : '2';

            case Key.D3:
                return shift ? '#' : '3';

            case Key.D4:
                return shift ? '$' : '4';

            case Key.D5:
                return shift ? '%' : '5';

            case Key.D6:
                return control ? 0x1E : shift ? '^' : '6';

            case Key.D7:
                return shift ? '&' : '7';

            case Key.D8:
                return shift ? '*' : '8';

            case Key.D9:
                return shift ? '(' : '9';

            case Key.D0:
                return shift ? ')' : '0';

            case Key.A:
                return control ? 0x01 : capsLock ? 'A' : 'a';

            case Key.B:
                return control ? 0x02 : capsLock ? 'B' : 'b';

            case Key.C:
                return control ? 0x03 : capsLock ? 'C' : 'c';

            case Key.D:
                return control ? 0x04 : capsLock ? 'D' : 'd';

            case Key.E:
                return control ? 0x05 : capsLock ? 'E' : 'e';

            case Key.F:
                return control ? 0x06 : capsLock ? 'F' : 'f';

            case Key.G:
                return control ? 0x07 : capsLock ? 'G' : 'g';

            case Key.H:
                return control ? 0x08 : capsLock ? 'H' : 'h';

            case Key.I:
                return control ? 0x09 : capsLock ? 'I' : 'i';

            case Key.J:
                return control ? 0x0A : capsLock ? 'J' : 'j';

            case Key.K:
                return control ? 0x0B : capsLock ? 'K' : 'k';

            case Key.L:
                return control ? 0x0C : capsLock ? 'L' : 'l';

            case Key.M:
                return control ? 0x0D : capsLock ? 'M' : 'm';

            case Key.N:
                return control ? 0x0E : capsLock ? 'N' : 'n';

            case Key.O:
                return control ? 0x0F : capsLock ? 'O' : 'o';

            case Key.P:
                return control ? 0x10 : capsLock ? 'P' : 'p';

            case Key.Q:
                return control ? 0x11 : capsLock ? 'Q' : 'q';

            case Key.R:
                return control ? 0x12 : capsLock ? 'R' : 'r';

            case Key.S:
                return control ? 0x13 : capsLock ? 'S' : 's';

            case Key.T:
                return control ? 0x14 : capsLock ? 'T' : 't';

            case Key.U:
                return control ? 0x15 : capsLock ? 'U' : 'u';

            case Key.V:
                return control ? 0x16 : capsLock ? 'V' : 'v';

            case Key.W:
                return control ? 0x17 : capsLock ? 'W' : 'w';

            case Key.X:
                return control ? 0x18 : capsLock ? 'X' : 'x';

            case Key.Y:
                return control ? 0x19 : capsLock ? 'Y' : 'y';

            case Key.Z:
                return control ? 0x1A : capsLock ? 'Z' : 'z';

            case Key.Unknown:
                switch (Environment.OSVersion.Platform)
                {
                case PlatformID.Win32NT:
                    switch (platformKeyCode)
                    {
                    case 0xBA: // WinForms Keys.Oem1
                        return shift ? ':' : ';';

                    case 0xBF: // WinForms Keys.Oem2
                        return shift ? '?' : '/';

                    case 0xC0: // WinForms Keys.Oem3
                        return shift ? '~' : '`';

                    case 0xDB: // WinForms Keys.Oem4
                        return shift ? '{' : '[';

                    case 0xDC: // WinForms Keys.Oem5
                        return control ? 0x1C : shift ? '|' : '\\';

                    case 0xDD: // WinForms Keys.Oem6
                        return control ? 0x1D : shift ? '}' : ']';

                    case 0xDE: // WinForms Keys.Oem7
                        return shift ? '"' : '\'';

                    case 0xBD: // WinForms Keys.OemMinus
                        return control ? 0x1F : shift ? '_' : '-';

                    case 0xBB: // WinForms Keys.OemPlus
                        return shift ? '+' : '=';

                    case 0xBC: // WinForms Keys.OemComma
                        return shift ? '<' : ',';

                    case 0xBE: // WinForms Keys.OemPeriod
                        return shift ? '>' : '.';
                    }
                    break;
#if !WINDOWS_PHONE
                case PlatformID.MacOSX:
                    switch (platformKeyCode)
                    {
                    case 0x29:
                        return shift ? ':' : ';';

                    case 0x2C:
                        return shift ? '?' : '/';

                    case 0x32:
                        return shift ? '~' : '`';

                    case 0x21:
                        return shift ? '{' : '[';

                    case 0x2A:
                        return control ? 0x1C : shift ? '|' : '\\';

                    case 0x1E:
                        return control ? 0x1D : shift ? '}' : ']';

                    case 0x27:
                        return shift ? '"' : '\'';

                    case 0x1B:
                        return control ? 0x1F : shift ? '_' : '-';

                    case 0x18:
                        return shift ? '+' : '=';

                    case 0x2B:
                        return shift ? '<' : ',';

                    case 0x2F:
                        return shift ? '>' : '.';
                    }
                    break;
#endif
                case PlatformID.Unix:
                    switch (platformKeyCode)
                    {
                    case 0x2F:
                        return shift ? ':' : ';';

                    case 0x3D:
                        return shift ? '?' : '/';

                    case 0x31:
                        return shift ? '~' : '`';

                    case 0x22:
                        return shift ? '{' : '[';

                    case 0x33:
                        return control ? 0x1C : shift ? '|' : '\\';

                    case 0x23:
                        return control ? 0x1D : shift ? '}' : ']';

                    case 0x30:
                        return shift ? '"' : '\'';

                    case 0x14:
                        return control ? 0x1F : shift ? '_' : '-';

                    case 0x15:
                        return shift ? '+' : '=';

                    case 0x3B:
                        return shift ? '<' : ',';

                    case 0x3C:
                        return shift ? '>' : '.';
                    }
                    break;
                }
                break;
            }

            return -1;
        }

        private static readonly Key[] KeyValues = 
            (from key in 
                 (from field in typeof(Key).GetFields() // missing Enum.GetValues; use reflection
                  where field.IsLiteral
                  select (Key)field.GetValue(typeof(Key)))
             where (key != Key.None) // filter Key.None
             select key).ToArray();

        private static readonly int KeyCount = (int)(KeyValues.Max()) + 1;

        private bool[] _states = new bool[KeyCount];
        private bool _capsLock;
        private bool _updateAnyKeyDown;
    }
}
