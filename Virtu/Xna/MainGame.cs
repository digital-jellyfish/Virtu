using Jellyfish.Library;
using Jellyfish.Virtu.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Jellyfish.Virtu
{
    public sealed class MainGame : GameBase
    {
        public MainGame() :
            base("Virtu")
        {
            Components.Add(new FrameRateCounter(this) { DrawOrder = 1, FontName = "Consolas" });
#if XBOX
            GraphicsDeviceManager.PreferredBackBufferWidth = 640;
            GraphicsDeviceManager.PreferredBackBufferHeight = 480;
#else
            GraphicsDeviceManager.PreferredBackBufferWidth = 560;
            GraphicsDeviceManager.PreferredBackBufferHeight = 384;
#endif
            _storageService = new XnaStorageService(this);
            _keyboardService = new XnaKeyboardService();
            _gamePortService = new XnaGamePortService();
            _audioService = new AudioService(); // not connected
            _videoService = new XnaVideoService(this);

            _machine = new Machine();
            _machine.Services.AddService(typeof(StorageService), _storageService);
            _machine.Services.AddService(typeof(KeyboardService), _keyboardService);
            _machine.Services.AddService(typeof(GamePortService), _gamePortService);
            _machine.Services.AddService(typeof(AudioService), _audioService);
            _machine.Services.AddService(typeof(VideoService), _videoService);
        }

        protected override void BeginRun()
        {
            _machine.Start();
        }

        protected override void Update(GameTime gameTime)
        {
            _keyboardService.Update();
            _gamePortService.Update();
            _audioService.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _videoService.Update();
            base.Draw(gameTime);
        }

        protected override void EndRun()
        {
            _machine.Stop();
        }

        private StorageService _storageService;
        private KeyboardService _keyboardService;
        private GamePortService _gamePortService;
        private AudioService _audioService;
        private VideoService _videoService;

        private Machine _machine;
    }
}
