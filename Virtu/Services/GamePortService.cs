namespace Jellyfish.Virtu.Services
{
    public struct Joystick
    {
        public Joystick(bool isUp, bool isLeft, bool isRight, bool isDown)
        {
            _isUp = isUp;
            _isLeft = isLeft;
            _isRight = isRight;
            _isDown = isDown;
        }

        public override bool Equals(object obj)
        {
            return ((obj is Joystick) && (this == (Joystick)obj));
        }

        public override int GetHashCode()
        {
            return (_isUp.GetHashCode() ^ _isLeft.GetHashCode() ^ _isRight.GetHashCode() ^ _isDown.GetHashCode());
        }

        public override string ToString()
        {
            return !(_isUp || _isDown || _isLeft || _isRight) ? "Position = Center" : 
                string.Concat("Position = ", _isUp ? "Up" : _isDown ? "Down" : string.Empty, _isLeft ? "Left" : _isRight ? "Right" : string.Empty);
        }

        public static bool operator ==(Joystick joystick1, Joystick joystick2)
        {
            return ((joystick1._isUp == joystick2._isUp) && (joystick1._isLeft == joystick2._isLeft) && 
                (joystick1._isRight == joystick2._isRight) && (joystick1._isDown == joystick2._isDown));
        }

        public static bool operator !=(Joystick joystick1, Joystick joystick2)
        {
            return !(joystick1 == joystick2);
        }

        public bool IsUp { get { return _isUp; } } // no auto props
        public bool IsLeft { get { return _isLeft; } }
        public bool IsRight { get { return _isRight; } }
        public bool IsDown { get { return _isDown; } }

        private bool _isUp;
        private bool _isLeft;
        private bool _isRight;
        private bool _isDown;
    }

    public class GamePortService : MachineService
    {
        public GamePortService(Machine machine) : 
            base(machine)
        {
            Paddle0 = Paddle1 = Paddle2 = Paddle3 = 255; // not connected
        }

        public virtual void Update() { }

        public int Paddle0 { get; protected set; }
        public int Paddle1 { get; protected set; }
        public int Paddle2 { get; protected set; }
        public int Paddle3 { get; protected set; }

        public Joystick Joystick0 { get; protected set; }
        public Joystick Joystick1 { get; protected set; }

        public bool IsButton0Down { get; protected set; }
        public bool IsButton1Down { get; protected set; }
        public bool IsButton2Down { get; protected set; }
    }
}
