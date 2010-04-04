using System;
using System.Runtime.InteropServices;
using Jellyfish.Library;

namespace Jellyfish.Virtu.Services
{
#if WINDOWS
    public sealed class XnaAudioService : AudioService
    {
        public XnaAudioService(Machine machine, GameBase game) : 
            base(machine)
        {
            if (game == null)
            {
                throw new ArgumentNullException("game");
            }

            _game = game;

            _directSound.Start(_game.Window.Handle);
            _directSound.Update += OnDirectSoundUpdate;
            _game.Exiting += (sender, e) => _directSound.Stop();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _directSound.Dispose();
            }
        }

        private void OnDirectSoundUpdate(object sender, DirectSoundUpdateEventArgs e) // audio thread
        {
            Update(e.BufferSize, (source, count) => Marshal.Copy(source, 0, e.Buffer, count));
        }

        private GameBase _game;
        private DirectSound _directSound = new DirectSound(SampleRate, SampleChannels, SampleBits, SampleSize);
    }
#endif
}
