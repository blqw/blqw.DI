using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace blqw
{
    public static class NamedServiceExtensions
    {
        public static IServiceCollection AddNamedScoped<TService>(this IServiceCollection services, string name)
            where TService : class
        {
            services.Add(new ServiceDescriptor(new NamedType(name, typeof(TService)), typeof(TService), ServiceLifetime.Scoped));
            return services;
        }
        public static IServiceCollection AddNamedScoped(this IServiceCollection services, string name, Func<IServiceProvider, object> implementationFactory)
        {
            services.Add(new ServiceDescriptor(new NamedType(name, null), implementationFactory, ServiceLifetime.Scoped));
            return services;
        }
        public static IServiceCollection AddNamedScoped(this IServiceCollection services, string name, Type serviceType)
        {
            services.Add(new ServiceDescriptor(new NamedType(name, serviceType), serviceType, ServiceLifetime.Scoped));
            return services;
        }


        public static IServiceCollection AddNamedSingleton<TService>(this IServiceCollection services, string name)
            where TService : class
        {
            services.Add(new ServiceDescriptor(new NamedType(name, typeof(TService)), typeof(TService), ServiceLifetime.Singleton));
            return services;
        }
        public static IServiceCollection AddNamedSingleton(this IServiceCollection services, string name, Func<IServiceProvider, object> implementationFactory)
        {
            services.Add(new ServiceDescriptor(new NamedType(name, null), implementationFactory, ServiceLifetime.Singleton));
            return services;
        }
        public static IServiceCollection AddNamedSingleton(this IServiceCollection services, string name, Type serviceType)
        {
            services.Add(new ServiceDescriptor(new NamedType(name, serviceType), serviceType, ServiceLifetime.Singleton));
            return services;
        }

        public static IServiceCollection AddNamedSingleton(this IServiceCollection services, string name, object service)
        {
            services.Add(new ServiceDescriptor(new NamedType(name, null), service));
            return services;
        }

        public static IServiceCollection AddNamedTransient<TService>(this IServiceCollection services, string name)
            where TService : class
        {
            services.Add(new ServiceDescriptor(new NamedType(name, typeof(TService)), typeof(TService), ServiceLifetime.Transient));
            return services;
        }
        public static IServiceCollection AddNamedTransient(this IServiceCollection services, string name, Func<IServiceProvider, object> implementationFactory)
        {
            services.Add(new ServiceDescriptor(new NamedType(name, null), implementationFactory, ServiceLifetime.Transient));
            return services;
        }
        public static IServiceCollection AddNamedTransient(this IServiceCollection services, string name, Type serviceType)
        {
            services.Add(new ServiceDescriptor(new NamedType(name, serviceType), serviceType, ServiceLifetime.Transient));
            return services;
        }
    }
}
