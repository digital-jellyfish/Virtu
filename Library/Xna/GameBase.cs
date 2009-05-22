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

            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            GraphicsDeviceService = (IGraphicsDeviceService)Services.GetService(typeof(IGraphicsDeviceService));

            Components.Add(new GamerServicesComponent(this));
            Content.RootDirectory = "Content";
            if (!string.IsNullOrEmpty(Name))
            {
                Window.Title = Name;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            if (gamePadState.Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }
            base.Update(gameTime);
        }

        public string Name { get; private set; }
        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
        public IGraphicsDeviceService GraphicsDeviceService { get; private set; }
    }
}
