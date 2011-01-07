using Jellyfish.Library;
using Jellyfish.Virtu.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Jellyfish.Virtu
{
    public sealed class MainGame : GameBase
    {
        public MainGame() : 
            base("Virtu")
        {
#if WINDOWS
            IsMouseVisible = true;
#endif
            var frameRateCounter = new FrameRateCounter(this); // no initializers; avoids CA2000
            Components.Add(frameRateCounter);
            frameRateCounter.DrawOrder = 1;
            frameRateCounter.FontName = "Consolas";

            _debugService = new DebugService(_machine);
#if WINDOWS_PHONE
            _storageService = new IsolatedStorageService(_machine);
#else
            _storageService = new XnaStorageService(_machine, this);
#endif
            _keyboardService = new XnaKeyboardService(_machine);
            _gamePortService = new XnaGamePortService(_machine);
            _audioService = new XnaAudioService(_machine, this);
            _videoService = new XnaVideoService(_machine, this);

            _machine.Services.AddService(typeof(DebugService), _debugService);
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
                _debugService.Dispose();
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
            var keyboardState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            var gamePadState = GamePad.GetState(PlayerIndex.One);
            var touches = TouchPanel.GetState();

            if (gamePadState.Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }

            _keyboardService.Update(ref keyboardState, ref gamePadState);
            _gamePortService.Update(ref gamePadState, ref touches);

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

        private DebugService _debugService;
        private StorageService _storageService;
        private XnaKeyboardService _keyboardService;
        private XnaGamePortService _gamePortService;
        private AudioService _audioService;
        private VideoService _videoService;
    }
}
