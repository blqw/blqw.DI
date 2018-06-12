using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace blqw
{
    /// <summary>
    /// 服务集合装饰器，主要用于替换和增强对于委托类型服务的支持
    /// </summary>
    internal class ServiceCollectionDecorator : IServiceCollection
    {
        private readonly IList<ServiceDescriptor> _serviceCollection;
        public ServiceCollectionDecorator(IList<ServiceDescriptor> serviceCollection) =>
            _serviceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));

        public int IndexOf(ServiceDescriptor item) => _serviceCollection.IndexOf(item);
        public void Insert(int index, ServiceDescriptor item) => _serviceCollection.Insert(index, item);
        public void RemoveAt(int index) => _serviceCollection.RemoveAt(index);

        public ServiceDescriptor this[int index]
        {
            get => _serviceCollection[index];
            set => _serviceCollection[index] = value;
        }

        public void Add(ServiceDescriptor item)
        {
            _serviceCollection.Add(item);
            //针对委托类型的服务, 新增一个MethodInfo类型的服务
            if (item.ImplementationInstance is Delegate && item.ServiceType != typeof(MethodInfo))
            {
                _serviceCollection.Add(new ServiceDescriptor(typeof(MethodInfo), ((Delegate)item.ImplementationInstance).Method));
            }
            else if (item.ImplementationFactory != null
                    && typeof(Delegate).IsAssignableFrom(item.ImplementationFactory.Method.ReturnType)
                    && item.ServiceType != typeof(MethodInfo))
            {
                _serviceCollection.Add(new ServiceDescriptor(typeof(MethodInfo), p => ((Delegate)item.ImplementationFactory(p)).Method, item.Lifetime));
            }
        }

        public void Clear() => _serviceCollection.Clear();
        public bool Contains(ServiceDescriptor item) => _serviceCollection.Contains(item);
        public void CopyTo(ServiceDescriptor[] array, int arrayIndex) => _serviceCollection.CopyTo(array, arrayIndex);
        public bool Remove(ServiceDescriptor item) => _serviceCollection.Remove(item);

        public int Count => _serviceCollection.Count;

        public bool IsReadOnly => _serviceCollection.IsReadOnly;

        public IEnumerator<ServiceDescriptor> GetEnumerator() => _serviceCollection.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _serviceCollection.GetEnumerator();
    }
}
