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

            _page = page;
            _media = media;
            _mediaSource = new WaveMediaStreamSource(SampleRate, SampleChannels, SampleBits, SampleSize, SampleLatency, OnMediaSourceUpdate);

            _page.Loaded += (sender, e) => { _media.SetSource(_mediaSource); _media.Play(); };
#if !WINDOWS_PHONE
            _page.Unloaded += (sender, e) => _media.Stop();
#endif
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
            //    _page.Dispatcher.BeginInvoke(() => 
            //    {
            //        ((MainPage)_page)._debug.Text += string.Concat(DateTime.Now, " OnMediaSourceUpdate", Environment.NewLine);
            //    });
            //}

            Update(bufferSize, (source, count) => Buffer.BlockCopy(source, 0, buffer, 0, count));
        }

        private UserControl _page;
        private MediaElement _media;
        private WaveMediaStreamSource _mediaSource;
        //private int _count;
    }
}
