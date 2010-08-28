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
#if !WINDOWS_PHONE
            page.Unloaded += (sender, e) => _media.Stop();
#endif
        }

        public override void SetVolume(double volume) // machine thread
        {
            _media.Dispatcher.BeginInvoke(() => _media.Volume = volume);
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

            Update(bufferSize, (source, count) => Buffer.BlockCopy(source, 0, buffer, 0, count));
        }

        private MediaElement _media;
        private WaveMediaStreamSource _mediaSource;
        //private int _count;
    }
}
