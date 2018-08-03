using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace blqw
{
    /// <summary>
    /// 命名服务扩展方法
    /// </summary>
    public static class NamedServiceExtensions
    {
        static IServiceCollection FluentAdd(this IServiceCollection services, ServiceDescriptor descriptor)
        {
            services?.Add(descriptor);
            return services;
        }

        #region Scoped
        public static IServiceCollection AddNamedScoped<TService>(this IServiceCollection services, string name)
          where TService : class =>
            services.FluentAdd(ServiceDescriptor.Scoped(NamedType.Get(name, typeof(TService)), typeof(TService)));

        public static IServiceCollection AddNamedScoped(this IServiceCollection services, string name, Func<IServiceProvider, object> implementationFactory) =>
            services.FluentAdd(ServiceDescriptor.Scoped(NamedType.Get(name, null), implementationFactory));
        public static IServiceCollection AddNamedScoped(this IServiceCollection services, string name, Type implementationType) =>
            services.FluentAdd(ServiceDescriptor.Scoped(NamedType.Get(name, implementationType), implementationType));
        #endregion

        #region Singleton
        public static IServiceCollection AddNamedSingleton<TService>(this IServiceCollection services, string name)
            where TService : class =>
            services.FluentAdd(ServiceDescriptor.Singleton(NamedType.Get(name, typeof(TService)), typeof(TService)));

        public static IServiceCollection AddNamedSingleton(this IServiceCollection services, string name, Func<IServiceProvider, object> implementationFactory) =>
            services.FluentAdd(ServiceDescriptor.Singleton(NamedType.Get(name, null), implementationFactory));

        public static IServiceCollection AddNamedSingleton(this IServiceCollection services, string name, Type implementationType) =>
            services.FluentAdd(ServiceDescriptor.Singleton(NamedType.Get(name, implementationType), implementationType));

        public static IServiceCollection AddNamedSingleton<TService>(this IServiceCollection services, string name, TService implementationInstance) =>
            services.FluentAdd(ServiceDescriptor.Singleton(NamedType.Get(name, typeof(TService)), implementationInstance));

        #endregion

        #region Transient
        public static IServiceCollection AddNamedTransient<TService>(this IServiceCollection services, string name) =>
            services.FluentAdd(ServiceDescriptor.Transient(NamedType.Get(name, typeof(TService)), typeof(TService)));
        public static IServiceCollection AddNamedTransient(this IServiceCollection services, string name, Func<IServiceProvider, object> implementationFactory) =>
            services.FluentAdd(ServiceDescriptor.Transient(NamedType.Get(name, null), implementationFactory));
        public static IServiceCollection AddNamedTransient(this IServiceCollection services, string name, Type implementationType) =>
            services.FluentAdd(ServiceDescriptor.Transient(NamedType.Get(name, implementationType), implementationType));
        #endregion

        #region GetService
        public static object GetNamedService(this IServiceProvider provider, string name) =>
            provider.GetService(NamedType.Get(name));
        public static object GetRequiredNamedService(this IServiceProvider provider, string name) =>
            provider.GetRequiredService(NamedType.Get(name));
        public static object GetRequiredNamedService(this IServiceProvider provider, string name, Type serviceType) =>
            provider.GetServices(NamedType.Get(name, serviceType)).LastOrDefault(serviceType.IsInstanceOfType);
        public static T GetRequiredNamedService<T>(this IServiceProvider provider, string name) =>
            provider.GetServices(NamedType.Get(name, typeof(T))).OfType<T>().Last<T>();
        public static T GetNamedService<T>(this IServiceProvider provider, string name) =>
            provider.GetServices(NamedType.Get(name, typeof(T))).OfType<T>().LastOrDefault<T>();
        public static IEnumerable<T> GetNamedServices<T>(this IServiceProvider provider, string name) =>
            provider.GetServices(NamedType.Get(name, typeof(T))).OfType<T>().ToArray();
        public static IEnumerable<object> GetNamedServices(this IServiceProvider provider, string name, Type serviceType) =>
            provider.GetServices(NamedType.Get(name, serviceType)).Where(serviceType.IsInstanceOfType).ToArray();
        #endregion
    }
}
