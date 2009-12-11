using System;
using System.Diagnostics.CodeAnalysis;
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

            _game.GraphicsDeviceManager.PreparingDeviceSettings += GraphicsDeviceManager_PreparingDeviceSettings;
            _game.GraphicsDeviceService.DeviceCreated += GraphicsDeviceService_DeviceCreated;
            _game.GraphicsDeviceService.DeviceReset += (sender, e) => SetTexturePosition();
        }

        [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "y*560")]
        public override void SetPixel(int x, int y, uint color)
        {
            _pixels[y * TextureWidth + x] = color;
            _pixelsDirty = true;
        }

        public override void Update()
        {
            if (_game.GraphicsDeviceManager.IsFullScreen != IsFullScreen)
            {
                if (IsFullScreen)
                {
                    _game.GraphicsDeviceManager.PreferredBackBufferWidth = _graphicsDevice.DisplayMode.Width; // avoids changing display mode
                    _game.GraphicsDeviceManager.PreferredBackBufferHeight = _graphicsDevice.DisplayMode.Height;
                }
                else
                {
                    _game.GraphicsDeviceManager.PreferredBackBufferWidth = TextureWidth;
                    _game.GraphicsDeviceManager.PreferredBackBufferHeight = TextureHeight;
                }
                _game.GraphicsDeviceManager.ToggleFullScreen();
            }

            if (_pixelsDirty)
            {
                _pixelsDirty = false;
                _texture.SetData(_pixels);
            }

            _spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
            _graphicsDevice.SamplerStates[0].MagFilter = TextureFilter.Point;
            _spriteBatch.Draw(_texture, _texturePosition, null, Color.White, 0f, Vector2.Zero, _textureScale, SpriteEffects.None, 0f);
            _spriteBatch.End();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _spriteBatch.Dispose();
                _texture.Dispose();
            }
        }

        private void GraphicsDeviceManager_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            DisplayMode displayMode = e.GraphicsDeviceInformation.Adapter.CurrentDisplayMode;
            PresentationParameters presentationParameters = e.GraphicsDeviceInformation.PresentationParameters;
            _textureScale = Math.Min(presentationParameters.BackBufferWidth / TextureWidth, presentationParameters.BackBufferHeight / TextureHeight);

            while ((presentationParameters.BackBufferWidth + TextureWidth <= displayMode.Width) && 
                (presentationParameters.BackBufferHeight + TextureHeight <= displayMode.Height))
            {
                presentationParameters.BackBufferWidth += TextureWidth;
                presentationParameters.BackBufferHeight += TextureHeight;
                _textureScale++;
            }
        }

        private void GraphicsDeviceService_DeviceCreated(object sender, EventArgs e)
        {
            _graphicsDevice = _game.GraphicsDevice;
            _spriteBatch = new SpriteBatch(_graphicsDevice);
            _texture = new Texture2D(_graphicsDevice, TextureWidth, TextureHeight, 1, TextureUsage.None, SurfaceFormat.Bgr32);
            _pixels = new uint[TextureWidth * TextureHeight];
            SetTexturePosition();
        }

        private void SetTexturePosition()
        {
            _texturePosition.X = (_graphicsDevice.PresentationParameters.BackBufferWidth - TextureWidth * _textureScale) / 2f; // centered
            _texturePosition.Y = (_graphicsDevice.PresentationParameters.BackBufferHeight - TextureHeight * _textureScale) / 2f;
        }

        private const int TextureWidth = 560;
        private const int TextureHeight = 384;

        private GameBase _game;
        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;
        private Texture2D _texture;
        private Vector2 _texturePosition;
        private int _textureScale;
        private uint[] _pixels;
        private bool _pixelsDirty;
    }
}
