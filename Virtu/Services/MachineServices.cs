using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Jellyfish.Library;
using Jellyfish.Virtu.Properties;

namespace Jellyfish.Virtu.Services
{
    public sealed class MachineServices : IServiceProvider
    {
        public void AddService(Type serviceType, MachineService serviceProvider)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }
            if (_serviceProviders.ContainsKey(serviceType))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.ServiceAlreadyPresent, serviceType.FullName), "serviceType");
            }
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }
            if (!serviceType.IsAssignableFrom(serviceProvider.GetType()))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.ServiceMustBeAssignable, serviceType.FullName, serviceProvider.GetType().FullName));
            }

            _serviceProviders.Add(serviceType, serviceProvider);
        }

        public void ForEach(Action<MachineService> action)
        {
            _serviceProviders.Values.ForEach(action);
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

        private Dictionary<Type, MachineService> _serviceProviders = new Dictionary<Type, MachineService>();
    }
}
