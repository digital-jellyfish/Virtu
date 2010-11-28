using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Jellyfish.Library;
using Jellyfish.Virtu.Services;

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

            JoystickDeadZone = 0.4;
        }

        public override void LoadState(BinaryReader reader, Version version)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            JoystickDeadZone = reader.ReadDouble();

            UseKeyboard = reader.ReadBoolean();
            Joystick0UpLeftKey = reader.ReadInt32();
            Joystick0UpKey = reader.ReadInt32();
            Joystick0UpRightKey = reader.ReadInt32();
            Joystick0LeftKey = reader.ReadInt32();
            Joystick0RightKey = reader.ReadInt32();
            Joystick0DownLeftKey = reader.ReadInt32();
            Joystick0DownKey = reader.ReadInt32();
            Joystick0DownRightKey = reader.ReadInt32();
            Joystick1UpLeftKey = reader.ReadInt32();
            Joystick1UpKey = reader.ReadInt32();
            Joystick1UpRightKey = reader.ReadInt32();
            Joystick1LeftKey = reader.ReadInt32();
            Joystick1RightKey = reader.ReadInt32();
            Joystick1DownLeftKey = reader.ReadInt32();
            Joystick1DownKey = reader.ReadInt32();
            Joystick1DownRightKey = reader.ReadInt32();
            Button0Key = reader.ReadInt32();
            Button1Key = reader.ReadInt32();
            Button2Key = reader.ReadInt32();
        }

        public override void SaveState(BinaryWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            writer.Write(JoystickDeadZone);

            writer.Write(UseKeyboard);
            writer.Write(Joystick0UpLeftKey);
            writer.Write(Joystick0UpKey);
            writer.Write(Joystick0UpRightKey);
            writer.Write(Joystick0LeftKey);
            writer.Write(Joystick0RightKey);
            writer.Write(Joystick0DownLeftKey);
            writer.Write(Joystick0DownKey);
            writer.Write(Joystick0DownRightKey);
            writer.Write(Joystick1UpLeftKey);
            writer.Write(Joystick1UpKey);
            writer.Write(Joystick1UpRightKey);
            writer.Write(Joystick1LeftKey);
            writer.Write(Joystick1RightKey);
            writer.Write(Joystick1DownLeftKey);
            writer.Write(Joystick1DownKey);
            writer.Write(Joystick1DownRightKey);
            writer.Write(Button0Key);
            writer.Write(Button1Key);
            writer.Write(Button2Key);
        }

        public bool ReadButton0()
        {
            return (_gamePortService.IsButton0Down || _keyboardService.IsOpenAppleKeyDown || 
                (UseKeyboard && (Button0Key > 0) && _keyboardService.IsKeyDown(Button0Key)));
        }

        public bool ReadButton1()
        {
            return (_gamePortService.IsButton1Down || _keyboardService.IsCloseAppleKeyDown || 
                (UseKeyboard && (Button1Key > 0) && _keyboardService.IsKeyDown(Button1Key)));
        }

        public bool ReadButton2()
        {
            return (_gamePortService.IsButton2Down || !_keyboardService.IsShiftKeyDown || // Shift' [TN9]
                (UseKeyboard && (Button2Key > 0) && _keyboardService.IsKeyDown(Button2Key)));
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public void TriggerTimers()
        {
            int paddle0 = _gamePortService.Paddle0;
            int paddle1 = _gamePortService.Paddle1;
            int paddle2 = _gamePortService.Paddle2;
            int paddle3 = _gamePortService.Paddle3;

            if (UseKeyboard) // override
            {
                if (((Joystick0UpLeftKey > 0) && _keyboardService.IsKeyDown(Joystick0UpLeftKey)) || 
                    ((Joystick0LeftKey > 0) && _keyboardService.IsKeyDown(Joystick0LeftKey)) || 
                    ((Joystick0DownLeftKey > 0) && _keyboardService.IsKeyDown(Joystick0DownLeftKey)))
                {
                    paddle0 -= 128;
                }
                if (((Joystick0UpRightKey > 0) && _keyboardService.IsKeyDown(Joystick0UpRightKey)) || 
                    ((Joystick0RightKey > 0) && _keyboardService.IsKeyDown(Joystick0RightKey)) || 
                    ((Joystick0DownRightKey > 0) && _keyboardService.IsKeyDown(Joystick0DownRightKey)))
                {
                    paddle0 += 128;
                }
                if (((Joystick0UpLeftKey > 0) && _keyboardService.IsKeyDown(Joystick0UpLeftKey)) || 
                    ((Joystick0UpKey > 0) && _keyboardService.IsKeyDown(Joystick0UpKey)) || 
                    ((Joystick0UpRightKey > 0) && _keyboardService.IsKeyDown(Joystick0UpRightKey)))
                {
                    paddle1 -= 128;
                }
                if (((Joystick0DownLeftKey > 0) && _keyboardService.IsKeyDown(Joystick0DownLeftKey)) || 
                    ((Joystick0DownKey > 0) && _keyboardService.IsKeyDown(Joystick0DownKey)) || 
                    ((Joystick0DownRightKey > 0) && _keyboardService.IsKeyDown(Joystick0DownRightKey)))
                {
                    paddle1 += 128;
                }
                if (((Joystick1UpLeftKey > 0) && _keyboardService.IsKeyDown(Joystick1UpLeftKey)) || 
                    ((Joystick1LeftKey > 0) && _keyboardService.IsKeyDown(Joystick1LeftKey)) || 
                    ((Joystick1DownLeftKey > 0) && _keyboardService.IsKeyDown(Joystick1DownLeftKey)))
                {
                    paddle2 -= 128;
                }
                if (((Joystick1UpRightKey > 0) && _keyboardService.IsKeyDown(Joystick1UpRightKey)) || 
                    ((Joystick1RightKey > 0) && _keyboardService.IsKeyDown(Joystick1RightKey)) || 
                    ((Joystick1DownRightKey > 0) && _keyboardService.IsKeyDown(Joystick1DownRightKey)))
                {
                    paddle2 += 128;
                }
                if (((Joystick1UpLeftKey > 0) && _keyboardService.IsKeyDown(Joystick1UpLeftKey)) || 
                    ((Joystick1UpKey > 0) && _keyboardService.IsKeyDown(Joystick1UpKey)) || 
                    ((Joystick1UpRightKey > 0) && _keyboardService.IsKeyDown(Joystick1UpRightKey)))
                {
                    paddle3 -= 128;
                }
                if (((Joystick1DownLeftKey > 0) && _keyboardService.IsKeyDown(Joystick1DownLeftKey)) || 
                    ((Joystick1DownKey > 0) && _keyboardService.IsKeyDown(Joystick1DownKey)) || 
                    ((Joystick1DownRightKey > 0) && _keyboardService.IsKeyDown(Joystick1DownRightKey)))
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

        public double JoystickDeadZone { get; set; }

        public bool UseKeyboard { get; set; }
        public int Joystick0UpLeftKey { get; set; }
        public int Joystick0UpKey { get; set; }
        public int Joystick0UpRightKey { get; set; }
        public int Joystick0LeftKey { get; set; }
        public int Joystick0RightKey { get; set; }
        public int Joystick0DownLeftKey { get; set; }
        public int Joystick0DownKey { get; set; }
        public int Joystick0DownRightKey { get; set; }
        public int Joystick1UpLeftKey { get; set; }
        public int Joystick1UpKey { get; set; }
        public int Joystick1UpRightKey { get; set; }
        public int Joystick1LeftKey { get; set; }
        public int Joystick1RightKey { get; set; }
        public int Joystick1DownLeftKey { get; set; }
        public int Joystick1DownKey { get; set; }
        public int Joystick1DownRightKey { get; set; }
        public int Button0Key { get; set; }
        public int Button1Key { get; set; }
        public int Button2Key { get; set; }

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
