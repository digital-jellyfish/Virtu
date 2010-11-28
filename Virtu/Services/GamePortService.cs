namespace Jellyfish.Virtu.Services
{
    public class GamePortService : MachineService
    {
        public GamePortService(Machine machine) : 
            base(machine)
        {
            Paddle0 = Paddle1 = Paddle2 = Paddle3 = 255; // not connected
        }

        public virtual void Update() { } // main thread

        public int Paddle0 { get; protected set; }
        public int Paddle1 { get; protected set; }
        public int Paddle2 { get; protected set; }
        public int Paddle3 { get; protected set; }

        public bool IsJoystick0Up { get; protected set; }
        public bool IsJoystick0Left { get; protected set; }
        public bool IsJoystick0Right { get; protected set; }
        public bool IsJoystick0Down { get; protected set; }

        public bool IsJoystick1Up { get; protected set; }
        public bool IsJoystick1Left { get; protected set; }
        public bool IsJoystick1Right { get; protected set; }
        public bool IsJoystick1Down { get; protected set; }

        public bool IsButton0Down { get; protected set; }
        public bool IsButton1Down { get; protected set; }
        public bool IsButton2Down { get; protected set; }
    }
}
