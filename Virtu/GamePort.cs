using System;
using System.Diagnostics.CodeAnalysis;
using Jellyfish.Library;
using Jellyfish.Virtu.Services;
using Jellyfish.Virtu.Settings;

namespace Jellyfish.Virtu
{
    public sealed class GamePort : MachineComponent
    {
        public GamePort(Machine machine) :
            base(machine)
        {
            _resetPaddle0StrobeEvent = ResetPaddle0StrobeEvent; // cache delegates; avoids garbage
            _resetPaddle1StrobeEvent = ResetPaddle1StrobeEvent;
            _resetPaddle2StrobeEvent = ResetPaddle2StrobeEvent;
            _resetPaddle3StrobeEvent = ResetPaddle3StrobeEvent;
        }

        public override void Initialize()
        {
            _keyboardService = Machine.Services.GetService<KeyboardService>();
            _gamePortService = Machine.Services.GetService<GamePortService>();
        }

        public bool ReadButton0()
        {
            GamePortSettings settings = Machine.Settings.GamePort;

            return (_gamePortService.IsButton0Down || _keyboardService.IsOpenAppleKeyDown || 
                (settings.UseKeyboard && (settings.Key.Button0 > 0) && _keyboardService.IsKeyDown(settings.Key.Button0)));
        }

        public bool ReadButton1()
        {
            GamePortSettings settings = Machine.Settings.GamePort;

            return (_gamePortService.IsButton1Down || _keyboardService.IsCloseAppleKeyDown || 
                (settings.UseKeyboard && (settings.Key.Button1 > 0) && _keyboardService.IsKeyDown(settings.Key.Button1)));
        }

        public bool ReadButton2()
        {
            GamePortSettings settings = Machine.Settings.GamePort;

            return (_gamePortService.IsButton2Down || 
                (settings.UseKeyboard && (settings.Key.Button2 > 0) && _keyboardService.IsKeyDown(settings.Key.Button2)));
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public void TriggerTimers()
        {
            int paddle0 = _gamePortService.Paddle0;
            int paddle1 = _gamePortService.Paddle1;
            int paddle2 = _gamePortService.Paddle2;
            int paddle3 = _gamePortService.Paddle3;

            GamePortSettings settings = Machine.Settings.GamePort;

            if (settings.UseKeyboard) // override
            {
                if (((settings.Key.Joystick0.UpLeft > 0) && _keyboardService.IsKeyDown(settings.Key.Joystick0.UpLeft)) || 
                    ((settings.Key.Joystick0.Left > 0) && _keyboardService.IsKeyDown(settings.Key.Joystick0.Left)) || 
                    ((settings.Key.Joystick0.DownLeft > 0) && _keyboardService.IsKeyDown(settings.Key.Joystick0.DownLeft)))
                {
                    paddle0 -= 128;
                }
                if (((settings.Key.Joystick0.UpRight > 0) && _keyboardService.IsKeyDown(settings.Key.Joystick0.UpRight)) || 
                    ((settings.Key.Joystick0.Right > 0) && _keyboardService.IsKeyDown(settings.Key.Joystick0.Right)) || 
                    ((settings.Key.Joystick0.DownRight > 0) && _keyboardService.IsKeyDown(settings.Key.Joystick0.DownRight)))
                {
                    paddle0 += 128;
                }
                if (((settings.Key.Joystick0.UpLeft > 0) && _keyboardService.IsKeyDown(settings.Key.Joystick0.UpLeft)) || 
                    ((settings.Key.Joystick0.Up > 0) && _keyboardService.IsKeyDown(settings.Key.Joystick0.Up)) || 
                    ((settings.Key.Joystick0.UpRight > 0) && _keyboardService.IsKeyDown(settings.Key.Joystick0.UpRight)))
                {
                    paddle1 -= 128;
                }
                if (((settings.Key.Joystick0.DownLeft > 0) && _keyboardService.IsKeyDown(settings.Key.Joystick0.DownLeft)) || 
                    ((settings.Key.Joystick0.Down > 0) && _keyboardService.IsKeyDown(settings.Key.Joystick0.Down)) || 
                    ((settings.Key.Joystick0.DownRight > 0) && _keyboardService.IsKeyDown(settings.Key.Joystick0.DownRight)))
                {
                    paddle1 += 128;
                }
                if (((settings.Key.Joystick1.UpLeft > 0) && _keyboardService.IsKeyDown(settings.Key.Joystick1.UpLeft)) || 
                    ((settings.Key.Joystick1.Left > 0) && _keyboardService.IsKeyDown(settings.Key.Joystick1.Left)) || 
                    ((settings.Key.Joystick1.DownLeft > 0) && _keyboardService.IsKeyDown(settings.Key.Joystick1.DownLeft)))
                {
                    paddle2 -= 128;
                }
                if (((settings.Key.Joystick1.UpRight > 0) && _keyboardService.IsKeyDown(settings.Key.Joystick1.UpRight)) || 
                    ((settings.Key.Joystick1.Right > 0) && _keyboardService.IsKeyDown(settings.Key.Joystick1.Right)) || 
                    ((settings.Key.Joystick1.DownRight > 0) && _keyboardService.IsKeyDown(settings.Key.Joystick1.DownRight)))
                {
                    paddle2 += 128;
                }
                if (((settings.Key.Joystick1.UpLeft > 0) && _keyboardService.IsKeyDown(settings.Key.Joystick1.UpLeft)) || 
                    ((settings.Key.Joystick1.Up > 0) && _keyboardService.IsKeyDown(settings.Key.Joystick1.Up)) || 
                    ((settings.Key.Joystick1.UpRight > 0) && _keyboardService.IsKeyDown(settings.Key.Joystick1.UpRight)))
                {
                    paddle3 -= 128;
                }
                if (((settings.Key.Joystick1.DownLeft > 0) && _keyboardService.IsKeyDown(settings.Key.Joystick1.DownLeft)) || 
                    ((settings.Key.Joystick1.Down > 0) && _keyboardService.IsKeyDown(settings.Key.Joystick1.Down)) || 
                    ((settings.Key.Joystick1.DownRight > 0) && _keyboardService.IsKeyDown(settings.Key.Joystick1.DownRight)))
                {
                    paddle3 += 128;
                }
            }

            Paddle0Strobe = true;
            Paddle1Strobe = true;
            Paddle2Strobe = true;
            Paddle3Strobe = true;

            Machine.Events.AddEvent(MathHelpers.ClampByte(paddle0) * CyclesPerValue, _resetPaddle0StrobeEvent); // [7-29]
            Machine.Events.AddEvent(MathHelpers.ClampByte(paddle1) * CyclesPerValue, _resetPaddle1StrobeEvent);
            Machine.Events.AddEvent(MathHelpers.ClampByte(paddle2) * CyclesPerValue, _resetPaddle2StrobeEvent);
            Machine.Events.AddEvent(MathHelpers.ClampByte(paddle3) * CyclesPerValue, _resetPaddle3StrobeEvent);
        }

        private void ResetPaddle0StrobeEvent()
        {
            Paddle0Strobe = false;
        }

        private void ResetPaddle1StrobeEvent()
        {
            Paddle1Strobe = false;
        }

        private void ResetPaddle2StrobeEvent()
        {
            Paddle2Strobe = false;
        }

        private void ResetPaddle3StrobeEvent()
        {
            Paddle3Strobe = false;
        }

        public bool Paddle0Strobe { get; private set; }
        public bool Paddle1Strobe { get; private set; }
        public bool Paddle2Strobe { get; private set; }
        public bool Paddle3Strobe { get; private set; }

        private const int CyclesPerValue = 11;

        private Action _resetPaddle0StrobeEvent;
        private Action _resetPaddle1StrobeEvent;
        private Action _resetPaddle2StrobeEvent;
        private Action _resetPaddle3StrobeEvent;

        private KeyboardService _keyboardService;
        private GamePortService _gamePortService;
    }
}
