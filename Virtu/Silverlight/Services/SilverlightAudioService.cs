using System.Windows.Controls;

namespace Jellyfish.Virtu.Services
{
    public sealed class SilverlightAudioService : AudioService
    {
        public SilverlightAudioService(Machine machine, UserControl page, MediaElement media) : 
            base(machine)
        {
            _page = page;
            _media = media;

            // TODO
        }

        private UserControl _page;
        private MediaElement _media;
    }
}
