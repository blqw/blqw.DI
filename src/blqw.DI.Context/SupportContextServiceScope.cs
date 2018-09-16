using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace blqw.DI
{
    class SupportContextServiceScope : IServiceScope
    {
        private readonly IServiceScope _scope;

        public SupportContextServiceScope(SupportContextServiceProvider parent, IServiceScope scope)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));
            ServiceProvider = new SupportContextServiceProvider(scope.ServiceProvider, parent);
        }

        public IServiceProvider ServiceProvider { get; }

        public void Dispose()
        {
            _scope.Dispose();
            ((SupportContextServiceProvider)ServiceProvider).Dispose();
        }

        ~SupportContextServiceScope()
        {
            ((SupportContextServiceProvider)ServiceProvider).Dispose();
        }
    }
}
