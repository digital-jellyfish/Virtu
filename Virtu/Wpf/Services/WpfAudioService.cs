using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using Jellyfish.Library;

namespace Jellyfish.Virtu.Services
{
    public sealed class WpfAudioService : AudioService
    {
        [SecurityCritical]
        public WpfAudioService(Machine machine, UserControl page) : 
            base(machine)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }

            _directSound = new DirectSound(SampleRate, SampleChannels, SampleBits, SampleSize, OnDirectSoundUpdate);

            page.Loaded += (sender, e) => 
            {
                var window = Window.GetWindow(page);
                _directSound.Start(window.GetHandle());
                window.Closed += (_sender, _e) => _directSound.Stop();
            };
        }

        public override void SetVolume(float volume)
        {
            _directSound.SetVolume(volume);
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

            Marshal.Copy(Source, 0, buffer, bufferSize);
            Update();
        }

        private DirectSound _directSound;
        //private int _count;
    }
}
