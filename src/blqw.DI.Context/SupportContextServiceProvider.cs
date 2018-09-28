using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace blqw.DI
{
    class SupportContextServiceProvider: IServiceProvider
    {
        private static long _sequence = -1;

        private static long CreateId()
        {
            unchecked
            {
                return Interlocked.Increment(ref _sequence);
            }
        }

        private readonly IServiceProvider _provider;

        public SupportContextServiceProvider Parent { get; }
        public SupportContextServiceProvider Root { get; }


        public long Id { get; }
        private SupportContextServiceProvider() => Accessor = new ServiceProviderAccessor(this);
        public SupportContextServiceProvider(IServiceProvider provider, SupportContextServiceProvider parent) : this()
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            Parent = parent;
            Root = parent?.Root ?? this;
            Id = CreateId();
            if (!ServiceContext.Push(this))
            {
                throw new InvalidOperationException();
            }
        }

        private int _disposed;
        public bool IsDisposed => _disposed > 0;
        internal void Dispose()
        {
            if (_disposed == 0 && Interlocked.Increment(ref _disposed) == 1)
            {
                ServiceContext.PopTo(Parent);
            }
        }

        public bool IsRoot => Parent == null;

        public bool IsMyParent(SupportContextServiceProvider provider)
        {
            for (var parent = Parent;
                     parent != null;
                     parent = parent.Parent)
            {
                if (ReferenceEquals(parent, provider))
                {
                    return true;
                }
            }
            return false;
        }


        public object GetService(Type serviceType)
        {
            var value = _provider.GetService(serviceType);
            if (value is IServiceScopeFactory factory)
            {
                return new SupportContextServiceScopeFactory(this, factory);
            }
            if (ReferenceEquals(value, _provider))
            {
                return this;
            }
            return value;
        }

        public override string ToString()
        {
            var list = new List<string>();
            for (var current = this;
                     current != null;
                     current = current.Parent)
            {
                list.Add(current.Id.ToString());
            }
            list.Reverse();
            return string.Join("-", list);
        }

        internal ServiceProviderAccessor Accessor { get; }
    }
}
