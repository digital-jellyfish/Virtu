﻿using System;
using System.Diagnostics.CodeAnalysis;
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
            TargetElapsedTime = TimeSpan.FromTicks(333333); // 30 fps
#elif XBOX
            Components.Add(new GamerServicesComponent(this));
#endif
            GraphicsDeviceService = (IGraphicsDeviceService)Services.GetService(typeof(IGraphicsDeviceService));

            if (!string.IsNullOrEmpty(Name))
            {
                Window.Title = Name;
            }
        }

        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        protected override void Update(GameTime gameTime)
        {
            var gamePadState = GamePad.GetState(PlayerIndex.One);
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
