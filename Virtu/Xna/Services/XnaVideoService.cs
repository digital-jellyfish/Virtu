using System;
using System.Diagnostics.CodeAnalysis;
#if WINDOWS
using System.Windows;
#endif
using Jellyfish.Library;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Jellyfish.Virtu.Services
{
    public sealed class XnaVideoService : VideoService
    {
        public XnaVideoService(Machine machine, GameBase game) : 
            base(machine)
        {
            if (game == null)
            {
                throw new ArgumentNullException("game");
            }

            _game = game;
#if WINDOWS || XBOX
            _game.GraphicsDeviceManager.PreparingDeviceSettings += OnGraphicsDeviceManagerPreparingDeviceSettings;
#endif
            _game.GraphicsDeviceService.DeviceCreated += OnGraphicsDeviceServiceDeviceCreated;
        }

        public override void SetFullScreen(bool isFullScreen)
        {
#if WINDOWS
            var graphicsDeviceManager = _game.GraphicsDeviceManager;
            if (graphicsDeviceManager.IsFullScreen != isFullScreen)
            {
                graphicsDeviceManager.IsFullScreen = isFullScreen;
                _game.SynchronizationContext.Send(state => graphicsDeviceManager.ApplyChanges(), null);
            }
#endif
        }

        [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow")]
        public override void SetPixel(int x, int y, uint color)
        {
            _pixels[y * TextureWidth + x] = (color & 0xFF00FF00) | ((color << 16) & 0x00FF0000) | ((color >> 16) & 0x000000FF); // RGBA
            _pixelsDirty = true;
        }

        public override void Update() // main thread
        {
            if (_pixelsDirty)
            {
                _pixelsDirty = false;
                _texture.SetData(_pixels);
            }

            var viewport = _graphicsDevice.Viewport;
            int scale = Math.Max(1, Math.Min(viewport.Width / TextureWidth, viewport.Height / TextureHeight));
            var position = new Vector2((viewport.Width - scale * TextureWidth) / 2, (viewport.Height - scale * TextureHeight) / 2);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp, null, null);
            _spriteBatch.Draw(_texture, position, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            _spriteBatch.End();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _spriteBatch.Dispose();
                _texture.Dispose();
            }

            base.Dispose(disposing);
        }

#if WINDOWS || XBOX
        private void OnGraphicsDeviceManagerPreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            var presentationParameters = e.GraphicsDeviceInformation.PresentationParameters;
#if WINDOWS
            if (!presentationParameters.IsFullScreen)
            {
                var maxScale = Math.Max(1, Math.Min((int)SystemParameters.FullPrimaryScreenWidth / TextureWidth, (int)SystemParameters.FullPrimaryScreenHeight / TextureHeight));
                presentationParameters.BackBufferWidth = maxScale * TextureWidth;
                presentationParameters.BackBufferHeight = maxScale * TextureHeight;
            }
            else
#endif
            {
                var displayMode = e.GraphicsDeviceInformation.Adapter.CurrentDisplayMode; // use display mode
                presentationParameters.BackBufferWidth = displayMode.Width;
                presentationParameters.BackBufferHeight = displayMode.Height;
            }
        }
#endif

        private void OnGraphicsDeviceServiceDeviceCreated(object sender, EventArgs e)
        {
            _graphicsDevice = _game.GraphicsDevice;
            _spriteBatch = new SpriteBatch(_graphicsDevice);
            _texture = new Texture2D(_graphicsDevice, TextureWidth, TextureHeight, false, SurfaceFormat.Color);
        }

        private const int TextureWidth = 560;
        private const int TextureHeight = 384;

        private GameBase _game;
        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;
        private Texture2D _texture;

        private uint[] _pixels = new uint[TextureWidth * TextureHeight];
        private bool _pixelsDirty;
    }
}
