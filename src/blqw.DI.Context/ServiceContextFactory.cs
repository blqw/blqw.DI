using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace blqw.DI
{
    /// <summary>
    /// 用于创建支持上下文的 <see cref="IServiceProvider"/>
    /// </summary>
    public static class ServiceContextFactory
    {
        /// <summary>
        /// 创建一个支持上下文的 <see cref="IServiceProvider"/>
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IServiceProvider Create(IServiceProvider provider) =>
            provider as SupportContextServiceProvider ?? new SupportContextServiceProvider(provider, null);

        /// <summary>
        /// 添加上下文服务，但依赖二次编译操作
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddContext(this IServiceCollection services) =>
            services.AddSingleton<IServiceProviderFactory<IServiceProvider>, ServiceProviderFactory>();

        private class ServiceProviderFactory : IServiceProviderFactory<IServiceProvider>
        {
            public IServiceProvider CreateBuilder(IServiceCollection services) => throw new NotImplementedException();

            public IServiceProvider CreateServiceProvider(IServiceProvider provider) => Create(provider);
        }
    }
}
