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
            _game = game;

            _directSound.Start(_game.Window.Handle);
            _directSound.Update += DirectSound_Update;
            _game.Exiting += (sender, e) => _directSound.Stop();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _directSound.Dispose();
            }
        }

        private void DirectSound_Update(object sender, DirectSoundUpdateEventArgs e)
        {
            IntPtr buffer = e.Buffer;
            Update(e.BufferSize, (source, count) => 
            {
                Marshal.Copy(source, 0, buffer, count);
                buffer = (IntPtr)((long)buffer + count);
            });
        }

        private GameBase _game;
        private DirectSound _directSound = new DirectSound(SampleRate, SampleChannels, SampleBits, SampleSize);
    }
#endif
}
