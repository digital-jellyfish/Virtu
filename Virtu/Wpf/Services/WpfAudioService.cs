using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using Jellyfish.Library;

namespace Jellyfish.Virtu.Services
{
    public sealed class WpfAudioService : AudioService
    {
        [SecurityCritical]
        public WpfAudioService(Machine machine, Window window) : 
            base(machine)
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }

            _window = window;
            _directSound = new DirectSound(SampleRate, SampleChannels, SampleBits, SampleSize, OnDirectSoundUpdate);

            _window.SourceInitialized += (sender, e) => _directSound.Start(_window.GetHandle());
            _window.Closed += (sender, e) => _directSound.Stop();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _directSound.Dispose();
            }

            base.Dispose(disposing);
        }

        private void OnDirectSoundUpdate(IntPtr buffer, int bufferSize) // audio thread
        {
            //if (_count++ % (1000 / SampleLatency) == 0)
            //{
            //    DebugService.WriteLine("OnDirectSoundUpdate");
            //}

            Update(bufferSize, (source, count) => Marshal.Copy(source, 0, buffer, count));
        }

        private Window _window;
        private DirectSound _directSound;
        //private int _count;
    }
}
