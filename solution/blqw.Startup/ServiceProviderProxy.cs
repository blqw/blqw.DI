using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace blqw
{
    /// <summary>
    /// 自定义服务提供程序的代理
    /// </summary>
    internal class ServiceProviderProxy : IServiceProvider
    {
        IServiceProvider _provider;
        private readonly object[] _extraServices;

        /// <summary>
        /// 构造一个服务提供程序的代理
        /// </summary>
        /// <param name="provider">被代理的服务提供程序</param>
        /// <param name="extraServices">额外提供的服务</param>
        public ServiceProviderProxy(IServiceProvider provider, object[] extraServices)
        {
            _provider = provider;
            _extraServices = extraServices;
        }

        /// <summary>
        /// 获取指定类型的服务
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
            //优先从 _extraServices 中获取服务, 如果没有则从 _provider 中获取服务
            var service = _extraServices?.FirstOrDefault(x => serviceType.IsInstanceOfType(x)) ?? _provider.GetService(serviceType);

            //如果没有获取到, 且参数 serviceType 是委托, 则尝试获取 MethodInfo 类型的服务, 通过名称匹配后创建委托
            if (service == null && typeof(Delegate).IsAssignableFrom(serviceType))
            {
                var methods = (_extraServices?.OfType<MethodInfo>() ?? Array.Empty<MethodInfo>()).Union(_provider.GetServices<MethodInfo>());
                var name = serviceType.Name;
                var serviceMethod = serviceType.GetMethod("Invoke");
                foreach (var method in methods)
                {
                    if (method.Name == name && CompareMethodSignature(serviceMethod, method) is MethodInfo m)
                    {
                        return m.CreateDelegate(serviceType);
                    }
                }
            }
            return service;
        }

        private MethodInfo CompareMethodSignature(MethodInfo method1, MethodInfo method2)
        {
            if (method1.ReturnType != method2.ReturnType)
            {
                return null;
            }
            var p1 = method1.GetParameters();
            var p2 = method2.GetParameters();
            if (p1.Length != p2.Length)
            {
                return null;
            }
            for (var i = 0; i < p1.Length; i++)
            {
                if (p1[i].ParameterType != p2[i].ParameterType)
                {
                    return null;
                }
            }
            return method2;
        }
    }
}
