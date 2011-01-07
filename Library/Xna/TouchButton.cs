using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace Jellyfish.Library
{
    public sealed class TouchButton : TouchRegion
    {
        public bool HasValue { get { return Touch.HasValue; } }
        public bool IsButtonDown { get { var touch = Touch.Value; return ((touch.State == TouchLocationState.Pressed) || (touch.State == TouchLocationState.Moved)); } }
        public Vector2 Position { get { return Touch.Value.Position; } }
    }
}
