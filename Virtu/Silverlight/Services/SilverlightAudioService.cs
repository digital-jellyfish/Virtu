using System;
using System.Windows.Controls;
using Jellyfish.Library;

namespace Jellyfish.Virtu.Services
{
    public sealed class SilverlightAudioService : AudioService
    {
        public SilverlightAudioService(Machine machine, UserControl page, MediaElement media) : 
            base(machine)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }
            if (media == null)
            {
                throw new ArgumentNullException("media");
            }

            _media = media;
            _mediaSource = new WaveMediaStreamSource(SampleRate, SampleChannels, SampleBits, SampleSize, SampleLatency, OnMediaSourceUpdate);
            _media.SetSource(_mediaSource);

            page.Loaded += (sender, e) => _media.Play();
            page.Unloaded += (sender, e) => _media.Stop();
        }

        public override void SetVolume(float volume)
        {
            _media.Dispatcher.Send(() => _media.Volume = volume);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _mediaSource.Dispose();
            }

            base.Dispose(disposing);
        }

        private void OnMediaSourceUpdate(byte[] buffer, int bufferSize) // audio thread
        {
            //if (_count++ % (1000 / SampleLatency) == 0)
            //{
            //    DebugService.WriteLine("OnMediaSourceUpdate");
            //}

            Buffer.BlockCopy(Source, 0, buffer, 0, bufferSize);
            Update();
        }

        private MediaElement _media;
        private WaveMediaStreamSource _mediaSource;
        //private int _count;
    }
}
