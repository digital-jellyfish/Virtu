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

            _page.Loaded += (sender, e) => _media.SetSource(_mediaSource);
            _mediaSource.Update += OnMediaSourceUpdate;
        }

        private void OnMediaSourceUpdate(object sender, WaveMediaStreamSourceUpdateEventArgs e)
        {
            int offset = 0;
            Update(e.BufferSize, (source, count) => 
            {
                Buffer.BlockCopy(source, 0, e.Buffer, offset, count);
                offset += count;
            });
        }

        private UserControl _page;
        private MediaElement _media;
        private WaveMediaStreamSource _mediaSource = new WaveMediaStreamSource(SampleRate, SampleChannels, SampleBits, SampleSize, SampleLatency);
    }
}
