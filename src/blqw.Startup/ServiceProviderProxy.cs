using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
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
            // 从 _provider 中获取服务
            var service = _provider.GetService(serviceType);
            // 如果获取不到服务, 则舱尝试获取委托服务
            if (service == null)
            {
                if (typeof(Delegate).IsAssignableFrom(serviceType))
                {
                    return CreateDelegate(serviceType, _provider.GetServices<MethodInfo>());
                }
                return null;
            }

            if (service is IList list && list.IsReadOnly == false && serviceType.IsGenericType && serviceType.GetGenericArguments().Length == 1)
            {
                var type = serviceType.GetGenericArguments()[0];
                var delegateType = (type as IServiceTypeDecorator)?.ServiceType ?? type;
                if (typeof(Delegate).IsAssignableFrom(delegateType))
                {
                    var delegateMethod = delegateType.GetMethod("Invoke");
                    for (var i = 0; i < list.Count; i++)
                    {
                        if (delegateType.IsInstanceOfType(list[i]))
                        {
                            continue;
                        }
                        var method = (list[i] as Delegate)?.Method ?? list[i] as MethodInfo;
                        if (CompareMethodSignature(delegateMethod, method) is MethodInfo m)
                        {
                            list[i] = m.CreateDelegate(delegateType);
                        }
                    }
                }
            }
            return service;
        }


        /// <summary>
        /// 创建委托
        /// </summary>
        private object CreateDelegate(Type delegateType, IEnumerable<MethodInfo> methods)
        {
            var delegateName = delegateType.Name;
            var delegateMethod = delegateType.GetMethod("Invoke");
            MethodInfo last = null;
            MethodInfo lastExact = null;
            foreach (var method in methods)
            {
                if (CompareMethodSignature(method, delegateMethod) is MethodInfo m)
                {
                    if (method.Name == delegateName)
                    {
                        lastExact = m;
                    }
                    else if (lastExact == null)
                    {
                        last = m;
                    }
                }
            }
            return (lastExact ?? last)?.CreateDelegate(delegateType);
        }

        /// <summary>
        /// 获取委托服务
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        private object CreateDelegateService(IServiceProvider provider, Type serviceType)
        {
            if (typeof(Delegate).IsAssignableFrom(serviceType) == false)
            {
                return null;
            }
            var methods = provider.GetServices<MethodInfo>();
            var name = serviceType.Name;
            var exportMethod = serviceType.GetMethod("Invoke");
            MethodInfo last = null;
            foreach (var method in methods)
            {
                if (CompareMethodSignature(method, exportMethod) is MethodInfo m)
                {
                    if (method.Name == name) // 如果能找到名称一致的方法就返回, 否则返回最后一个签名一致的方法
                    {
                        return m.CreateDelegate(serviceType);
                    }
                    last = m;
                }
            }
            return last?.CreateDelegate(serviceType);
            throw new NotImplementedException();
        }

        /// <summary>
        /// 比较2个方法签名是否相同, 如果相同返回方法2
        /// </summary>
        /// <param name="method1">方法1</param>
        /// <param name="method2">方法2</param>
        /// <returns></returns>
        private MethodInfo CompareMethodSignature(MethodInfo method1, MethodInfo method2)
        {
            if (method1 == null || method2 == null || method1.ReturnType != method2.ReturnType)
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
