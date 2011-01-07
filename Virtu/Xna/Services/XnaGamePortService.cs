using System.Diagnostics.CodeAnalysis;
using Jellyfish.Library;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Jellyfish.Virtu.Services
{
    public sealed class XnaGamePortService : GamePortService
    {
        public XnaGamePortService(Machine machine) : 
            base(machine)
        {
            _touchRegions = new TouchRegionCollection { _touchJoystick0, _touchJoystick1, _touchButton0, _touchButton1, _touchButton2 };
        }

        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference")]
        public void Update(ref GamePadState gamePadState, ref TouchCollection touches) // main thread
        {
            _lastState = _state;
            _state = gamePadState;

            var gamePort = Machine.GamePort;

            if (_state.IsConnected && (_state != _lastState))
            {
                var left = _state.ThumbSticks.Left;
                var right = _state.ThumbSticks.Right;
                var dpad = _state.DPad;
                var buttons = _state.Buttons;

                Paddle0 = (int)((1 + left.X) * GamePort.PaddleScale);
                Paddle1 = (int)((1 - left.Y) * GamePort.PaddleScale); // invert y
                Paddle2 = (int)((1 + right.X) * GamePort.PaddleScale);
                Paddle3 = (int)((1 - right.Y) * GamePort.PaddleScale); // invert y

                IsJoystick0Up = ((left.Y > gamePort.JoystickDeadZone) || (dpad.Up == ButtonState.Pressed));
                IsJoystick0Left = ((left.X < -gamePort.JoystickDeadZone) || (dpad.Left == ButtonState.Pressed));
                IsJoystick0Right = ((left.X > gamePort.JoystickDeadZone) || (dpad.Right == ButtonState.Pressed));
                IsJoystick0Down = ((left.Y < -gamePort.JoystickDeadZone) || (dpad.Down == ButtonState.Pressed));

                IsJoystick1Up = (right.Y > gamePort.JoystickDeadZone);
                IsJoystick1Left = (right.X < -gamePort.JoystickDeadZone);
                IsJoystick1Right = (right.X > gamePort.JoystickDeadZone);
                IsJoystick1Down = (right.Y < -gamePort.JoystickDeadZone);

                IsButton0Down = ((buttons.A == ButtonState.Pressed) || (buttons.LeftShoulder == ButtonState.Pressed));
                IsButton1Down = ((buttons.B == ButtonState.Pressed) || (buttons.RightShoulder == ButtonState.Pressed));
                IsButton2Down = (buttons.X == ButtonState.Pressed);
            }

            if (gamePort.UseTouch) // override
            {
                UpdateTouch(ref touches);
            }

            base.Update();
        }

        private void UpdateTouch(ref TouchCollection touches)
        {
            var gamePort = Machine.GamePort;

            _touchJoystick0.SetBounds(gamePort.Joystick0TouchX, gamePort.Joystick0TouchY, gamePort.Joystick0TouchWidth, gamePort.Joystick0TouchHeight);
            _touchJoystick0.SetRadius(gamePort.Joystick0TouchRadius);
            _touchJoystick0.KeepLast = gamePort.Joystick0TouchKeepLast;
            _touchJoystick0.Order = gamePort.Joystick0TouchOrder;
            _touchJoystick1.SetBounds(gamePort.Joystick1TouchX, gamePort.Joystick1TouchY, gamePort.Joystick1TouchWidth, gamePort.Joystick1TouchHeight);
            _touchJoystick1.SetRadius(gamePort.Joystick1TouchRadius);
            _touchJoystick1.KeepLast = gamePort.Joystick1TouchKeepLast;
            _touchJoystick1.Order = gamePort.Joystick1TouchOrder;
            _touchButton0.SetBounds(gamePort.Button0TouchX, gamePort.Button0TouchY, gamePort.Button0TouchWidth, gamePort.Button0TouchHeight);
            _touchButton0.Order = gamePort.Button0TouchOrder;
            _touchButton1.SetBounds(gamePort.Button1TouchX, gamePort.Button1TouchY, gamePort.Button1TouchWidth, gamePort.Button1TouchHeight);
            _touchButton1.Order = gamePort.Button1TouchOrder;
            _touchButton2.SetBounds(gamePort.Button2TouchX, gamePort.Button2TouchY, gamePort.Button2TouchWidth, gamePort.Button2TouchHeight);
            _touchButton2.Order = gamePort.Button2TouchOrder;

            _touchRegions.Update(ref touches);

            if (_touchJoystick0.HasValue)
            {
                var joystick = _touchJoystick0.GetJoystick();

                Paddle0 = (int)((1 + joystick.X) * GamePort.PaddleScale);
                Paddle1 = (int)((1 - joystick.Y) * GamePort.PaddleScale); // invert y

                IsJoystick0Up = (joystick.Y > gamePort.JoystickDeadZone);
                IsJoystick0Left = (joystick.X < -gamePort.JoystickDeadZone);
                IsJoystick0Right = (joystick.X > gamePort.JoystickDeadZone);
                IsJoystick0Down = (joystick.Y < -gamePort.JoystickDeadZone);
            }
            if (_touchJoystick1.HasValue)
            {
                var joystick = _touchJoystick1.GetJoystick();

                Paddle2 = (int)((1 + joystick.X) * GamePort.PaddleScale);
                Paddle3 = (int)((1 - joystick.Y) * GamePort.PaddleScale); // invert y

                IsJoystick1Up = (joystick.Y > gamePort.JoystickDeadZone);
                IsJoystick1Left = (joystick.X < -gamePort.JoystickDeadZone);
                IsJoystick1Right = (joystick.X > gamePort.JoystickDeadZone);
                IsJoystick1Down = (joystick.Y < -gamePort.JoystickDeadZone);
            }
            if (_touchButton0.HasValue)
            {
                IsButton0Down = _touchButton0.IsButtonDown;
            }
            if (_touchButton1.HasValue)
            {
                IsButton1Down = _touchButton1.IsButtonDown;
            }
            if (_touchButton2.HasValue)
            {
                IsButton2Down = _touchButton2.IsButtonDown;
            }
        }

        private GamePadState _state;
        private GamePadState _lastState;
        private TouchJoystick _touchJoystick0 = new TouchJoystick();
        private TouchJoystick _touchJoystick1 = new TouchJoystick();
        private TouchButton _touchButton0 = new TouchButton();
        private TouchButton _touchButton1 = new TouchButton();
        private TouchButton _touchButton2 = new TouchButton();
        private TouchRegionCollection _touchRegions;
    }
}
