using System.Deployment.Application;
using System.IO.IsolatedStorage;

namespace Jellyfish.Virtu.Services
{
    public sealed class WpfStorageService : IsolatedStorageService
    {
        public WpfStorageService(Machine machine) : 
            base(machine)
        {
        }

        protected override IsolatedStorageFile GetStore()
        {
            return ApplicationDeployment.IsNetworkDeployed ? // clickonce
                IsolatedStorageFile.GetUserStoreForApplication() : IsolatedStorageFile.GetUserStoreForAssembly();
        }
    }
}
