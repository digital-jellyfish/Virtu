using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace Jellyfish.Library
{
    public sealed class TouchJoystick : TouchRegion
    {
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public Vector2 GetJoystick()
        {
            TouchLocation touch;
            bool isValid;
            if (KeepLast && LastTouch.HasValue)
            {
                touch = LastTouch.Value;
                isValid = (touch.State != TouchLocationState.Invalid);
            }
            else
            {
                touch = Touch.Value;
                isValid = (touch.State == TouchLocationState.Pressed) || (touch.State == TouchLocationState.Moved);
            }
            if (isValid)
            {
                var center = Center;
                var joystick = new Vector2((touch.Position.X - center.X) / Radius, (center.Y - touch.Position.Y) / Radius);
                if (joystick.LengthSquared() > 1)
                {
                    joystick.Normalize();
                }
                return joystick;
            }

            return Vector2.Zero;
        }

        public void SetRadius(float radius) // scaled
        {
            Radius = (int)(radius * Math.Min(TouchPanel.DisplayWidth, TouchPanel.DisplayHeight));
        }

        public int Radius { get; set; }
        public bool KeepLast { get; set; }

        public Vector2 Center { get { return FirstTouch.Value.Position; } }
        public bool HasValue { get { return ((KeepLast && LastTouch.HasValue) || Touch.HasValue); } }
        public Vector2 Position { get { return (KeepLast && LastTouch.HasValue) ? LastTouch.Value.Position : Touch.Value.Position; } }
    }
}
