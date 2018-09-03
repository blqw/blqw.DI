using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace blqw.DI
{
    public static class DelegateServiceProvdierExtensions
    {
        /// <summary>
        /// 构造支持委托转换的服务提供程序
        /// </summary>
        public static IServiceProvider BuildSupportDelegateServiceProvdier(this IServiceCollection services) =>
            services.SupportDelegate(x => x.BuildServiceProvider());

        /// <summary>
        /// 可自己控制构造过程的 支持委托转换的服务提供程序
        /// </summary>
        public static IServiceProvider SupportDelegate(this IServiceCollection services, Func<IServiceCollection, IServiceProvider> build)
        {
            var methodServices = new List<ServiceDescriptor>();
            // 循环所有服务
            foreach (var item in services)
            {
                if (typeof(Delegate).IsAssignableFrom(item.ServiceType))
                {
                    // 针对委托类型的服务做一次处理
                    if (item.ImplementationInstance is Delegate)
                    {
                        methodServices.Add(new ServiceDescriptor(typeof(MethodInfo), ((Delegate)item.ImplementationInstance).Method));
                    }
                    else if (item.ImplementationFactory != null)
                    {
                        methodServices.Add(new ServiceDescriptor(typeof(MethodInfo), p => ((Delegate)item.ImplementationFactory(p)).Method, item.Lifetime));
                    }
                }
            }
            methodServices.ForEach(services.Add);
            var provider = build(services);
            return new DelegateServiceProvdier(provider);
        }
    }
}
