using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace blqw
{
    internal class StartupServiceCollection : IServiceCollection
    {
        ServiceCollection _serviceCollection;
        public StartupServiceCollection() => _serviceCollection = new ServiceCollection();

        public int IndexOf(ServiceDescriptor item) => ((IServiceCollection)_serviceCollection).IndexOf(item);
        public void Insert(int index, ServiceDescriptor item) => ((IServiceCollection)_serviceCollection).Insert(index, item);
        public void RemoveAt(int index) => ((IServiceCollection)_serviceCollection).RemoveAt(index);

        public ServiceDescriptor this[int index] { get => ((IServiceCollection)_serviceCollection)[index]; set => ((IServiceCollection)_serviceCollection)[index] = value; }

        public void Add(ServiceDescriptor item)
        {
            if (item.ImplementationInstance is Delegate && item.ServiceType != typeof(MethodInfo))
            {
                ((IServiceCollection)_serviceCollection).Add(new ServiceDescriptor(
                       typeof(MethodInfo),
                       ((Delegate)item.ImplementationInstance).Method));
            }
            else if (item.ImplementationFactory != null 
                    && typeof(Delegate).IsAssignableFrom(item.ImplementationFactory.Method.ReturnType) 
                    && item.ServiceType != typeof(MethodInfo))
            {
                ((IServiceCollection)_serviceCollection).Add(new ServiceDescriptor(
                           typeof(MethodInfo),
                           p => ((Delegate)item.ImplementationFactory(p)).Method,
                           item.Lifetime));
            }
            ((IServiceCollection)_serviceCollection).Add(item);
        }

        public void Clear() => ((IServiceCollection)_serviceCollection).Clear();
        public bool Contains(ServiceDescriptor item) => ((IServiceCollection)_serviceCollection).Contains(item);
        public void CopyTo(ServiceDescriptor[] array, int arrayIndex) => ((IServiceCollection)_serviceCollection).CopyTo(array, arrayIndex);
        public bool Remove(ServiceDescriptor item) => ((IServiceCollection)_serviceCollection).Remove(item);

        public int Count => ((IServiceCollection)_serviceCollection).Count;

        public bool IsReadOnly => ((IServiceCollection)_serviceCollection).IsReadOnly;

        public IEnumerator<ServiceDescriptor> GetEnumerator() => ((IServiceCollection)_serviceCollection).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IServiceCollection)_serviceCollection).GetEnumerator();
    }
}
