using System;
using System.Runtime.InteropServices;
using System.Windows;
using Jellyfish.Library;

namespace Jellyfish.Virtu.Services
{
    public sealed class WpfAudioService : AudioService
    {
        public WpfAudioService(Machine machine, Window window) : 
            base(machine)
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }

            _window = window;

            _window.SourceInitialized += (sender, e) => _directSound.Start(_window.GetHandle());
            _directSound.Update += OnDirectSoundUpdate;
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

        private void OnDirectSoundUpdate(object sender, DirectSoundUpdateEventArgs e)
        {
            IntPtr buffer = e.Buffer;
            Update(e.BufferSize, (source, count) => 
            {
                Marshal.Copy(source, 0, buffer, count);
                buffer = (IntPtr)((long)buffer + count);
            });
        }

        private Window _window;
        private DirectSound _directSound = new DirectSound(SampleRate, SampleChannels, SampleBits, SampleSize);
    }
}
