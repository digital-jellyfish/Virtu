using System;
using System.Threading;
using Microsoft.Xna.Framework;
#if XBOX
using Microsoft.Xna.Framework.GamerServices;
#endif
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Jellyfish.Library
{
    public class GameBase : Game
    {
        public GameBase(string name)
        {
            Name = name;

            Content.RootDirectory = "Content";
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
#if WINDOWS_PHONE
            GraphicsDeviceManager.IsFullScreen = true;
            GraphicsDeviceManager.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            TargetElapsedTime = TimeSpan.FromTicks(333333); // 30 fps
#elif XBOX
            GraphicsDeviceManager.IsFullScreen = true;
            Components.Add(new GamerServicesComponent(this));
#elif WINDOWS
            SynchronizationContext = new System.Windows.Forms.WindowsFormsSynchronizationContext();
#endif
            GraphicsDeviceService = (IGraphicsDeviceService)Services.GetService(typeof(IGraphicsDeviceService));

            if (!string.IsNullOrEmpty(Name))
            {
                Window.Title = Name;
            }
        }

        public string Name { get; private set; }
        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
        public IGraphicsDeviceService GraphicsDeviceService { get; private set; }
#if WINDOWS
        public SynchronizationContext SynchronizationContext { get; private set; }
#endif
    }
}
