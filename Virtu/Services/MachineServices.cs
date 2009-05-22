using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Jellyfish.Virtu.Properties;

namespace Jellyfish.Virtu.Services
{
    public class MachineServices : IServiceProvider
    {
        public void AddService(Type serviceType, object serviceProvider)
        {
            if (_serviceProviders.ContainsKey(serviceType))
            {
                throw new ArgumentException(SR.ServiceAlreadyPresentFormat(serviceType.FullName), "serviceType");
            }
            if (!serviceType.IsAssignableFrom(serviceProvider.GetType()))
            {
                throw new ArgumentException(SR.ServiceMustBeAssignableFormat(serviceType.FullName, serviceProvider.GetType().FullName));
            }

            _serviceProviders.Add(serviceType, serviceProvider);
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        public object GetService(Type serviceType)
        {
            return _serviceProviders.ContainsKey(serviceType) ? _serviceProviders[serviceType] : null;
        }

        public void RemoveService(Type serviceType)
        {
            _serviceProviders.Remove(serviceType);
        }

        private Dictionary<Type, object> _serviceProviders = new Dictionary<Type, object>();
    }
}
