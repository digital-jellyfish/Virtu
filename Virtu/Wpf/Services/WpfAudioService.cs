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

        private void OnDirectSoundUpdate(object sender, DirectSoundUpdateEventArgs e) // audio thread
        {
            //if (_count++ % (1000 / SampleLatency) == 0)
            //{
            //    _window.Dispatcher.BeginInvoke(() => 
            //    {
            //        ((MainWindow)_window)._debug.Text += string.Concat(DateTime.Now, " OnDirectSoundUpdate", Environment.NewLine);
            //    });
            //}

            Update(e.BufferSize, (source, count) => Marshal.Copy(source, 0, e.Buffer, count));
        }

        private Window _window;
        private DirectSound _directSound = new DirectSound(SampleRate, SampleChannels, SampleBits, SampleSize);
        //private int _count;
    }
}
