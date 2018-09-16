using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace blqw.DI
{
    class SupportContextServiceScopeFactory : IServiceScopeFactory
    {
        private readonly SupportContextServiceProvider _provider;
        private readonly IServiceScopeFactory _factory;

        public SupportContextServiceScopeFactory(SupportContextServiceProvider provider, IServiceScopeFactory factory)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public IServiceScope CreateScope() => new SupportContextServiceScope(_provider, _factory.CreateScope());
    }
}
