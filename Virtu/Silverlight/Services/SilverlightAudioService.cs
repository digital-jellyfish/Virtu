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

            _page.Loaded += (sender, e) => { _media.SetSource(_mediaSource); _media.Play(); };
            _mediaSource.Update += OnMediaSourceUpdate;
            //_page.Closed += (sender, e) => _media.Stop(); // SL is missing Closed / Unloaded event
        }

        private void OnMediaSourceUpdate(object sender, WaveMediaStreamSourceUpdateEventArgs e) // audio thread
        {
            //if (_count++ % (1000 / SampleLatency) == 0)
            //{
            //    _page.Dispatcher.BeginInvoke(new Action(() =>
            //    {
            //        ((MainPage)_page)._debug.Text += string.Concat(DateTime.Now, " OnMediaSourceUpdate", Environment.NewLine);
            //    }));
            //}

            Update(e.BufferSize, (source, count) => 
            {
                Buffer.BlockCopy(source, 0, e.Buffer, 0, count);
            });
        }

        private UserControl _page;
        private MediaElement _media;
        private WaveMediaStreamSource _mediaSource = new WaveMediaStreamSource(SampleRate, SampleChannels, SampleBits, SampleSize, SampleLatency);
        //private int _count;
    }
}
