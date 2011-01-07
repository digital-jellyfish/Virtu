using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Input.Touch;

namespace Jellyfish.Library
{
    public sealed class TouchRegionCollection : Collection<TouchRegion>
    {
        public TouchRegionCollection()
        {
        }

        public void Update(ref TouchCollection touches)
        {
            for (int i = 0; i < base.Count; i++)
            {
                base[i].Touch = null;
            }

            for (int i = 0; i < touches.Count; i++)
            {
                var touch = touches[i];
                if (UpdateById(ref touch))
                {
                    continue;
                }

                TouchLocation prevTouch;
                if (touch.TryGetPreviousLocation(out prevTouch))
                {
                    UpdateByPosition(ref prevTouch);
                }
                else
                {
                    UpdateByPosition(ref touch);
                }
            }
        }

        private bool UpdateById(ref TouchLocation touch)
        {
            for (int i = 0; i < base.Count; i++)
            {
                var region = base[i];
                if (region.LastTouch.HasValue && (region.LastTouch.Value.Id == touch.Id))
                {
                    region.Touch = touch;
                    region.LastTouch = touch;
                    return true;
                }
            }

            return false;
        }

        private void UpdateByPosition(ref TouchLocation touch)
        {
            if ((touch.State == TouchLocationState.Pressed) || (touch.State == TouchLocationState.Moved))
            {
                TouchRegion topMostRegion = null;

                for (int i = 0; i < base.Count; i++)
                {
                    var region = base[i];
                    if (region.Bounds.Contains((int)touch.Position.X, (int)touch.Position.Y) && ((topMostRegion == null) || (topMostRegion.Order < region.Order)))
                    {
                        topMostRegion = region;
                    }
                }

                if ((topMostRegion != null) && !topMostRegion.Touch.HasValue)
                {
                    topMostRegion.Touch = touch;
                    topMostRegion.FirstTouch = touch;
                    topMostRegion.LastTouch = touch;
                }
            }
        }
    }
}
