using System.Security;
using Jellyfish.Library;

namespace Jellyfish.Virtu
{
    public sealed partial class MainApp : ApplicationBase
    {
        [SecurityCritical]
        public MainApp() :
            base("Virtu")
        {
        }
    }
}
