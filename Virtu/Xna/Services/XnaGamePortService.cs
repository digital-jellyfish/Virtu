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
                Vector2 left = _state.ThumbSticks.Left;
                Vector2 right = _state.ThumbSticks.Right;
                GamePadDPad dpad = _state.DPad;

                Paddle0 = (int)((1f + left.X) * PaddleScale);
                Paddle1 = (int)((1f - left.Y) * PaddleScale); // invert y
                Paddle2 = (int)((1f + right.X) * PaddleScale);
                Paddle3 = (int)((1f - right.Y) * PaddleScale); // invert y

                Joystick0 = GetJoystick(ref left, ref dpad);
                Joystick1 = GetJoystick(ref right);

                IsButton0Down = ((_state.Buttons.A == ButtonState.Pressed) || (_state.Buttons.LeftShoulder == ButtonState.Pressed));
                IsButton1Down = ((_state.Buttons.B == ButtonState.Pressed) || (_state.Buttons.RightShoulder == ButtonState.Pressed));
                IsButton2Down = (_state.Buttons.X == ButtonState.Pressed);
            }
        }

        private static Joystick GetJoystick(ref Vector2 thumbstick)
        {
            bool isUp = (thumbstick.Y > JoystickDeadZone);
            bool isLeft = (thumbstick.X < -JoystickDeadZone);
            bool isRight = (thumbstick.X > JoystickDeadZone);
            bool isDown = (thumbstick.Y < -JoystickDeadZone);

            return new Joystick(isUp, isLeft, isRight, isDown);
        }

        private static Joystick GetJoystick(ref Vector2 thumbstick, ref GamePadDPad dpad)
        {
            bool isUp = ((thumbstick.Y > JoystickDeadZone) || (dpad.Up == ButtonState.Pressed));
            bool isLeft = ((thumbstick.X < -JoystickDeadZone) || (dpad.Left == ButtonState.Pressed));
            bool isRight = ((thumbstick.X > JoystickDeadZone) || (dpad.Right == ButtonState.Pressed));
            bool isDown = ((thumbstick.Y < -JoystickDeadZone) || (dpad.Down == ButtonState.Pressed));

            return new Joystick(isUp, isLeft, isRight, isDown);
        }

        private const float PaddleScale = 128f;
        private const float JoystickDeadZone = 0.5f;

        private GamePadState _state;
        private GamePadState _lastState;
    }
}
