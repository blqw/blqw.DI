﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace blqw
{
    /// <summary>
    /// 自定义服务提供程序的代理
    /// </summary>
    internal class ServiceProviderProxy : IServiceProvider
    {
        IServiceProvider _provider;

        /// <summary>
        /// 构造一个服务提供程序的代理
        /// </summary>
        /// <param name="provider">被代理的服务提供程序</param>
        public ServiceProviderProxy(IServiceProvider provider) => _provider = provider ?? throw new ArgumentNullException(nameof(provider));

        /// <summary>
        /// 获取指定类型的服务
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
            if (typeof(IServiceProvider) == serviceType)
            {
                return this;
            }
            //从 _provider 中获取服务
            var service = _provider.GetService(serviceType);

            //如果没有获取到, 且参数 serviceType 是委托, 则尝试获取 MethodInfo 类型的服务, 通过名称匹配后创建委托
            if (service == null && typeof(Delegate).IsAssignableFrom(serviceType))
            {
                var methods = _provider.GetServices<MethodInfo>();
                var name = serviceType.Name;
                var serviceMethod = serviceType.GetMethod("Invoke");
                MethodInfo last = null;
                foreach (var method in methods)
                {
                    if (CompareMethodSignature(serviceMethod, method) is MethodInfo m)
                    {
                        if (method.Name == name)
                        {
                            return m.CreateDelegate(serviceType);
                        }
                        last = m;
                    }
                }
                return last?.CreateDelegate(serviceType);
            }

            if (service is IEnumerable enumerable && serviceType.IsGenericType && serviceType.GetGenericArguments().Length == 1)
            {
                var serType = serviceType.GetGenericArguments()[0];
                var name = serType.Name;
                if (serType is NamedType namedType)
                {
                    serType = namedType.ExportType;
                    name = null;
                }
                if (typeof(Delegate).IsAssignableFrom(serType) && enumerable is IList list && list.IsReadOnly == false)
                {
                    var serviceMethod = serType.GetMethod("Invoke");
                    for (var i = 0; i < list.Count; i++)
                    {
                        var method = (list[0] as Delegate)?.Method ?? list[0] as MethodInfo;
                        if (method != null && CompareMethodSignature(serviceMethod, method) is MethodInfo m)
                        {
                            if (method.Name == name || name == null)
                            {
                                list[i] = m.CreateDelegate(serType);
                            }
                        }
                    }
                    return service;
                }
                return null;
            }

            return service;
        }

        /// <summary>
        /// 比较2个方法签名是否相同, 如果相同返回方法2
        /// </summary>
        /// <param name="method1">方法1</param>
        /// <param name="method2">方法2</param>
        /// <returns></returns>
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
