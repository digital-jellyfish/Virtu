using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace Jellyfish.Library
{
    public class TouchRegion
    {
        public void SetBounds(float x, float y, float width, float height) // scaled
        {
            Bounds = new Rectangle((int)(x * TouchPanel.DisplayWidth), (int)(y * TouchPanel.DisplayHeight), (int)(width * TouchPanel.DisplayWidth), (int)(height * TouchPanel.DisplayHeight));
        }

        public Rectangle Bounds { get; set; }
        public int Order { get; set; }

        internal TouchLocation? Touch { get; set; }
        internal TouchLocation? FirstTouch { get; set; }
        internal TouchLocation? LastTouch { get; set; }
    }
}
