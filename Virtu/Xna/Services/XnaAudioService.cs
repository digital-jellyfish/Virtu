using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using Jellyfish.Library;

namespace Jellyfish.Virtu.Services
{
    public sealed class XnaAudioService : AudioService
    {
        [SuppressMessage("Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges")]
        public XnaAudioService(Machine machine, GameBase game) : 
            base(machine)
        {
            if (game == null)
            {
                throw new ArgumentNullException("game");
            }

            _game = game;

#if WINDOWS
            _directSound.Start(_game.Window.Handle);
            _directSound.Update += OnDirectSoundUpdate;
            _game.Exiting += (sender, e) => _directSound.Stop();
#else
            _timer = new Timer(OnTimerUpdate, null, 0, SampleLatency);
            _game.Exiting += (sender, e) => _timer.Change(Timeout.Infinite, Timeout.Infinite);
#endif
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
#if WINDOWS
                _directSound.Dispose();
#else
                _timer.Dispose();
#endif
            }
        }

#if WINDOWS
        private void OnDirectSoundUpdate(object sender, DirectSoundUpdateEventArgs e) // audio thread
        {
            Update(e.BufferSize, (source, count) => Marshal.Copy(source, 0, e.Buffer, count));
        }
#else
        private void OnTimerUpdate(object state) // audio thread
        {
            Update(0, null);
        }
#endif

        private GameBase _game;
#if WINDOWS
        private DirectSound _directSound = new DirectSound(SampleRate, SampleChannels, SampleBits, SampleSize);
#else
        private Timer _timer;
#endif
    }
}
