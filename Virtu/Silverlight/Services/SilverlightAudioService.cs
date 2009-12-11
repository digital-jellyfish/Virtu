using System;
using System.Windows.Controls;

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

            // TODO
        }

        private UserControl _page;
        private MediaElement _media;
    }
}
