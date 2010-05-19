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

            _game.GraphicsDeviceManager.PreparingDeviceSettings += OnGraphicsDeviceManagerPreparingDeviceSettings;
            _game.GraphicsDeviceService.DeviceCreated += OnGraphicsDeviceServiceDeviceCreated;
            _game.GraphicsDeviceService.DeviceReset += (sender, e) => SetTexturePosition();
        }

        [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow")]
        public override void SetPixel(int x, int y, uint color)
        {
            uint rgbaColor = ((color << 16) & 0xFF0000) | (color & 0x00FF00) | ((color >> 16) & 0x0000FF); // convert from BGRA to RGBA
            _pixels[y * TextureWidth + x] = rgbaColor;
            _pixels[(y + 1) * TextureWidth + x] = (_textureScale < 1) ? rgbaColor : 0x0;
            _pixelsDirty = true;
        }

        public override void Update() // main thread
        {
#if WINDOWS
            if (_game.GraphicsDeviceManager.IsFullScreen != IsFullScreen)
            {
                _game.GraphicsDeviceManager.ToggleFullScreen();
            }
#endif
            if (_pixelsDirty)
            {
                _pixelsDirty = false;
                _texture.SetData(_pixels);
            }

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp, null, null);
            _spriteBatch.Draw(_texture, _texturePosition, null, Color.White, 0, Vector2.Zero, _textureScale, SpriteEffects.None, 0);
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

        private void OnGraphicsDeviceManagerPreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            var displayMode = e.GraphicsDeviceInformation.Adapter.CurrentDisplayMode;
            var presentationParameters = e.GraphicsDeviceInformation.PresentationParameters;

#if WINDOWS_PHONE
            bool portraitOrientation = (presentationParameters.DisplayOrientation & DisplayOrientation.Portrait) != 0;
            if (portraitOrientation)
            {
                _textureScale = Math.Min((float)displayMode.TitleSafeArea.Width / TextureWidth, (float)displayMode.TitleSafeArea.Height / TextureHeight);
                presentationParameters.BackBufferWidth = displayMode.Width; // always use portrait display mode
                presentationParameters.BackBufferHeight = displayMode.Height;
            }
            else
            {
                _textureScale = Math.Min((float)displayMode.TitleSafeArea.Height / TextureWidth, (float)displayMode.TitleSafeArea.Width / TextureHeight);
                presentationParameters.BackBufferWidth = displayMode.Height; // always use landscape display mode
                presentationParameters.BackBufferHeight = displayMode.Width;
            }
            if (_textureScale > 1)
            {
                _textureScale = (float)Math.Floor(_textureScale); // integer scale up
            }
#elif XBOX
            _textureScale = Math.Min(displayMode.TitleSafeArea.Width / TextureWidth, displayMode.TitleSafeArea.Height / TextureHeight);
            presentationParameters.BackBufferWidth = displayMode.Width; // always use display mode
            presentationParameters.BackBufferHeight = displayMode.Height;
#else
            if (presentationParameters.IsFullScreen)
            {
                _textureScale = Math.Min((int)SystemParameters.PrimaryScreenWidth / TextureWidth, (int)SystemParameters.PrimaryScreenHeight / TextureHeight);
                presentationParameters.BackBufferWidth = displayMode.Width; // avoids changing display mode
                presentationParameters.BackBufferHeight = displayMode.Height;
            }
            else
            {
                _textureScale = Math.Min((int)SystemParameters.FullPrimaryScreenWidth / TextureWidth, (int)SystemParameters.FullPrimaryScreenHeight / TextureHeight);
                presentationParameters.BackBufferWidth = (int)_textureScale * TextureWidth;
                presentationParameters.BackBufferHeight = (int)_textureScale * TextureHeight;
            }
#endif
        }

        private void OnGraphicsDeviceServiceDeviceCreated(object sender, EventArgs e)
        {
            _graphicsDevice = _game.GraphicsDevice;
            _spriteBatch = new SpriteBatch(_graphicsDevice);
            _texture = new Texture2D(_graphicsDevice, TextureWidth, TextureHeight, false, SurfaceFormat.Color);
            _pixels = new uint[TextureWidth * TextureHeight];
            SetTexturePosition();
        }

        private void SetTexturePosition()
        {
            _texturePosition.X = (_graphicsDevice.PresentationParameters.BackBufferWidth - TextureWidth * _textureScale) / 2; // centered
            _texturePosition.Y = (_graphicsDevice.PresentationParameters.BackBufferHeight - TextureHeight * _textureScale) / 2;
        }

        private const int TextureWidth = 560;
        private const int TextureHeight = 384;

        private GameBase _game;
        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;
        private Texture2D _texture;
        private Vector2 _texturePosition;
        private float _textureScale;
        private uint[] _pixels;
        private bool _pixelsDirty;
    }
}
