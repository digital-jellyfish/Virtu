using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Jellyfish.Virtu.Services
{
    public sealed class XnaGamePortService : GamePortService
    {
        public XnaGamePortService(Machine machine) : 
            base(machine)
        {
        }

        public override void Update() // main thread
        {
            _lastState = _state;
            _state = GamePad.GetState(PlayerIndex.One);

            if (_state.IsConnected && (_state != _lastState))
            {
                var left = _state.ThumbSticks.Left;
                var right = _state.ThumbSticks.Right;
                var dpad = _state.DPad;
                var gamePort = Machine.GamePort;

                Paddle0 = (int)((1 + left.X) * PaddleScale);
                Paddle1 = (int)((1 - left.Y) * PaddleScale); // invert y
                Paddle2 = (int)((1 + right.X) * PaddleScale);
                Paddle3 = (int)((1 - right.Y) * PaddleScale); // invert y

                IsJoystick0Up = ((left.Y > gamePort.JoystickDeadZone) || (dpad.Up == ButtonState.Pressed));
                IsJoystick0Left = ((left.X < -gamePort.JoystickDeadZone) || (dpad.Left == ButtonState.Pressed));
                IsJoystick0Right = ((left.X > gamePort.JoystickDeadZone) || (dpad.Right == ButtonState.Pressed));
                IsJoystick0Down = ((left.Y < -gamePort.JoystickDeadZone) || (dpad.Down == ButtonState.Pressed));

                IsJoystick1Up = (right.Y > gamePort.JoystickDeadZone);
                IsJoystick1Left = (right.X < -gamePort.JoystickDeadZone);
                IsJoystick1Right = (right.X > gamePort.JoystickDeadZone);
                IsJoystick1Down = (right.Y < -gamePort.JoystickDeadZone);

                IsButton0Down = ((_state.Buttons.A == ButtonState.Pressed) || (_state.Buttons.LeftShoulder == ButtonState.Pressed));
                IsButton1Down = ((_state.Buttons.B == ButtonState.Pressed) || (_state.Buttons.RightShoulder == ButtonState.Pressed));
                IsButton2Down = (_state.Buttons.X == ButtonState.Pressed);
            }
        }

        private const int PaddleScale = 128;

        private GamePadState _state;
        private GamePadState _lastState;
    }
}
