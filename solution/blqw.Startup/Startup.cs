using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace blqw
{
    /// <summary>
    /// 
    /// </summary>
    public static class Startup
    {
        private static readonly ServiceDescriptor _serviceProcessed = new ServiceDescriptor(new { }.GetType(), p => null, ServiceLifetime.Transient);
        private static List<StartupInvoker> _invokers;
        private static IServiceCollection _services;
        private static IServiceProvider _serviceProvider;
        private static Func<Type, bool> _filter;

        private static List<StartupInvoker> Invokers => _invokers 
                                    ?? (_invokers = AppDomain.CurrentDomain.GetAssemblies()
                                        .SelectMany(x => x.Modules)
                                        .SelectMany(x => x.GetTypes())
                                        .Where(x => x.IsClass && x.IsAbstract && x.IsSealed)
                                        .Where(x => x.Name == "Startup" && !ReferenceEquals(x, typeof(Startup)))
                                        .Where(x => _filter == null || _filter(x))
                                        .Select(x => new StartupInvoker(x))
                                        .Where(x => x.Type != null)
                                        .ToList());

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="container"></param>
        public static void ConfigureServices(IServiceCollection services, Func<Type, bool> filter = null)
        {
            if (services == null)
            {
                services = _services ?? (_services = new StartupServiceCollection());
            }
            if (services.Contains(_serviceProcessed))
            {
                return;
            }
            services.Add(_serviceProcessed);
            _filter = filter;
            Invokers.ForEach(x => x.ConfigureServices(services));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="filter"></param>
        public static void Configure(IServiceProvider serviceProvider, params object[] args)
        {
            if (serviceProvider == null)
            {
                if (_services == null)
                {
                    throw new ArgumentNullException(nameof(_services));
                }
                serviceProvider = _serviceProvider ?? (_serviceProvider = _services.BuildServiceProvider());
            }
            var provider = new ServiceProviderProxy(serviceProvider, args);
            Invokers.ForEach(x => x.Configure(provider));
        }
    }
}
