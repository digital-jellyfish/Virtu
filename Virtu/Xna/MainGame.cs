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

            _storageService = new XnaStorageService(_machine, this);
            _keyboardService = new XnaKeyboardService(_machine);
            _gamePortService = new XnaGamePortService(_machine);
            _audioService = new XnaAudioService(_machine, this);
            _videoService = new XnaVideoService(_machine, this);

            _machine.Services.AddService(typeof(StorageService), _storageService);
            _machine.Services.AddService(typeof(KeyboardService), _keyboardService);
            _machine.Services.AddService(typeof(GamePortService), _gamePortService);
            _machine.Services.AddService(typeof(AudioService), _audioService);
            _machine.Services.AddService(typeof(VideoService), _videoService);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _machine.Dispose();
                _storageService.Dispose();
                _keyboardService.Dispose();
                _gamePortService.Dispose();
                _audioService.Dispose();
                _videoService.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void BeginRun()
        {
            _machine.Start();
        }

        protected override void Update(GameTime gameTime)
        {
            _keyboardService.Update();
            _gamePortService.Update();

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

        private Machine _machine = new Machine();

        private StorageService _storageService;
        private KeyboardService _keyboardService;
        private GamePortService _gamePortService;
        private AudioService _audioService;
        private VideoService _videoService;
    }
}
