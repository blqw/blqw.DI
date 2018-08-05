using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace blqw.DI
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
        /// <summary>
        /// 添加命名服务
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="name">服务名称</param>
        public static IServiceCollection AddNamedScoped<TService>(this IServiceCollection services, string name)
          where TService : class =>
            services.FluentAdd(ServiceDescriptor.Scoped(NamedType.Get(name, typeof(TService)), typeof(TService)));
        /// <summary>
        /// 添加命名服务
        /// </summary>
        /// <param name="name">服务名称</param>
        public static IServiceCollection AddNamedScoped(this IServiceCollection services, string name, Func<IServiceProvider, object> implementationFactory) =>
            services.FluentAdd(ServiceDescriptor.Scoped(NamedType.Get(name, null), implementationFactory));
        /// <summary>
        /// 添加命名服务
        /// </summary>
        /// <param name="name">服务名称</param>
        public static IServiceCollection AddNamedScoped(this IServiceCollection services, string name, Type implementationType) =>
            services.FluentAdd(ServiceDescriptor.Scoped(NamedType.Get(name, implementationType), implementationType));
        #endregion

        #region Singleton
        /// <summary>
        /// 添加命名服务
        /// </summary>
        /// <param name="name">服务名称</param>
        public static IServiceCollection AddNamedSingleton<TService>(this IServiceCollection services, string name)
            where TService : class =>
            services.FluentAdd(ServiceDescriptor.Singleton(NamedType.Get(name, typeof(TService)), typeof(TService)));

        /// <summary>
        /// 添加命名服务
        /// </summary>
        /// <param name="name">服务名称</param>
        public static IServiceCollection AddNamedSingleton(this IServiceCollection services, string name, Func<IServiceProvider, object> implementationFactory) =>
            services.FluentAdd(ServiceDescriptor.Singleton(NamedType.Get(name, null), implementationFactory));

        /// <summary>
        /// 添加命名服务
        /// </summary>
        /// <param name="name">服务名称</param>
        public static IServiceCollection AddNamedSingleton(this IServiceCollection services, string name, Type implementationType) =>
            services.FluentAdd(ServiceDescriptor.Singleton(NamedType.Get(name, implementationType), implementationType));

        /// <summary>
        /// 添加命名服务
        /// </summary>
        /// <param name="name">服务名称</param>
        public static IServiceCollection AddNamedSingleton<TService>(this IServiceCollection services, string name, TService implementationInstance) =>
            services.FluentAdd(ServiceDescriptor.Singleton(NamedType.Get(name, typeof(TService)), implementationInstance));

        #endregion

        #region Transient
        /// <summary>
        /// 添加命名服务
        /// </summary>
        /// <param name="name">服务名称</param>
        public static IServiceCollection AddNamedTransient<TService>(this IServiceCollection services, string name) =>
            services.FluentAdd(ServiceDescriptor.Transient(NamedType.Get(name, typeof(TService)), typeof(TService)));
        /// <summary>
        /// 添加命名服务
        /// </summary>
        /// <param name="name">服务名称</param>
        public static IServiceCollection AddNamedTransient(this IServiceCollection services, string name, Func<IServiceProvider, object> implementationFactory) =>
            services.FluentAdd(ServiceDescriptor.Transient(NamedType.Get(name, null), implementationFactory));
        /// <summary>
        /// 添加命名服务
        /// </summary>
        /// <param name="name">服务名称</param>
        public static IServiceCollection AddNamedTransient(this IServiceCollection services, string name, Type implementationType) =>
            services.FluentAdd(ServiceDescriptor.Transient(NamedType.Get(name, implementationType), implementationType));
        #endregion

        #region GetService
        /// <summary>
        /// 获取命名服务
        /// </summary>
        /// <param name="name">服务名称</param>
        public static object GetNamedService(this IServiceProvider provider, string name) =>
            provider.GetService(NamedType.Get(name));
        /// <summary>
        /// 获取命名服务
        /// </summary>
        /// <param name="name">服务名称</param>
        public static object GetRequiredNamedService(this IServiceProvider provider, string name) =>
            provider.GetRequiredService(NamedType.Get(name));
        /// <summary>
        /// 获取命名服务
        /// </summary>
        /// <param name="name">服务名称</param>
        public static object GetRequiredNamedService(this IServiceProvider provider, string name, Type serviceType) =>
            provider.GetServices(NamedType.Get(name, serviceType)).Last(serviceType.IsInstanceOfType);
        /// <summary>
        /// 获取命名服务
        /// </summary>
        /// <param name="name">服务名称</param>
        public static T GetRequiredNamedService<T>(this IServiceProvider provider, string name) =>
            (T)provider.GetRequiredService(NamedType.Get(name, typeof(T)));
        /// <summary>
        /// 获取命名服务
        /// </summary>
        /// <param name="name">服务名称</param>
        public static T GetNamedService<T>(this IServiceProvider provider, string name) =>
            (T)provider.GetService(NamedType.Get(name, typeof(T)));
        /// <summary>
        /// 获取命名服务
        /// </summary>
        /// <param name="name">服务名称</param>
        public static IEnumerable<T> GetNamedServices<T>(this IServiceProvider provider, string name) =>
            provider.GetServices(NamedType.Get(name, typeof(T))).OfType<T>().ToArray();
        /// <summary>
        /// 获取命名服务
        /// </summary>
        /// <param name="name">服务名称</param>
        public static IEnumerable<object> GetNamedServices(this IServiceProvider provider, string name, Type serviceType) =>
            provider.GetServices(NamedType.Get(name, serviceType)).Where(serviceType.IsInstanceOfType).ToArray();
        #endregion
    }
}
