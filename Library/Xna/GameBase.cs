using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Jellyfish.Library
{
    public class GameBase : Game
    {
        public GameBase() : 
            this(null)
        {
        }

        public GameBase(string name)
        {
            Name = name;

            Content.RootDirectory = "Content";
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
#if WINDOWS_PHONE
            GraphicsDeviceManager.IsFullScreen = true;
            TargetElapsedTime = TimeSpan.FromTicks(333333); // 30 fps
#elif XBOX
            Components.Add(new GamerServicesComponent(this));
#else
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
