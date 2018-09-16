using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace blqw.DI
{
    /// <summary>
    /// 服务集合
    /// </summary>
    internal class ServiceCollection : IServiceCollection
    {
        private readonly IList<ServiceDescriptor> _serviceCollection;
        public ServiceCollection(IList<ServiceDescriptor> serviceCollection) =>
            _serviceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));

        public int IndexOf(ServiceDescriptor item) => _serviceCollection.IndexOf(item);
        public void Insert(int index, ServiceDescriptor item) => _serviceCollection.Insert(index, item);
        public void RemoveAt(int index) => _serviceCollection.RemoveAt(index);

        public ServiceDescriptor this[int index]
        {
            get => _serviceCollection[index];
            set => _serviceCollection[index] = value;
        }

        public void Add(ServiceDescriptor item) => _serviceCollection.Add(item);

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
