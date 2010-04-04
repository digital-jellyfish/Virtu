using System.Security.Permissions;
using Jellyfish.Library;

namespace Jellyfish.Virtu
{
    public sealed partial class MainApp : ApplicationBase
    {
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public MainApp() :
            base("Virtu")
        {
        }
    }
}
