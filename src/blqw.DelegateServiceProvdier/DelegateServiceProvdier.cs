using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace blqw.DI
{
    /// <summary>
    /// 可创建委托服务的提供程序
    /// </summary>
    internal class DelegateServiceProvdier : IServiceProvider
    {
        IServiceProvider _provider;
        ILogger _logger;
        /// <summary>
        /// 构造一个服务提供程序的代理
        /// </summary>
        /// <param name="provider">被代理的服务提供程序</param>
        public DelegateServiceProvdier(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = _provider.GetService<ILoggerFactory>()?.CreateLogger<DelegateServiceProvdier>();
        }

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
                return _services.GetOrAdd(serviceType, x =>
                {
                    if (typeof(Delegate).IsAssignableFrom(x))
                    {
                        var p = _provider.GetService<IServiceProvider>();
                        return CreateDelegate(x, _provider.GetServices<MethodInfo>());
                    }
                    return null;
                });
            }

            if (service is IList list && list.IsReadOnly == false && serviceType.IsGenericType && serviceType.GetGenericArguments().Length == 1)
            {
                var type = serviceType.GetGenericArguments()[0];
                if (list.Count == 0 && typeof(Delegate).IsAssignableFrom(type))
                {
                    list = _provider.GetServices<MethodInfo>() as IList;
                }
                var delegateType = (type as IServiceProvider)?.GetService(typeof(Type)) as Type ?? type;
                serviceType = typeof(IEnumerable<>).MakeGenericType(delegateType);
                return _services.GetOrAdd(serviceType, x =>
                {
                    var newServices = new ArrayList();
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
                            if (CompareMethodSignature(delegateMethod, method))
                            {
                                newServices.Add(method.CreateDelegate(delegateType, null));
                            }
                        }
                    }
                    return newServices.ToArray(delegateType);
                });
            }
            return service;
        }

        ConcurrentDictionary<Type, object> _services = new ConcurrentDictionary<Type, object>();


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
                if (CompareMethodSignature(method, delegateMethod))
                {
                    if (method.Name == delegateName)
                    {
                        lastExact = method;
                    }
                    else if (lastExact == null)
                    {
                        last = method;
                    }
                }
            }
            try
            {
                return (lastExact ?? last).CreateDelegate(delegateType, null);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ex.Message);
                return null;
            }
        }


        /// <summary>
        /// 比较2个方法签名是否相同
        /// </summary>
        /// <param name="method1">方法1</param>
        /// <param name="method2">方法2</param>
        /// <returns></returns>
        private bool CompareMethodSignature(MethodInfo method1, MethodInfo method2)
        {
            if (method1 == null || method2 == null || method1.ReturnType != method2.ReturnType)
            {
                return false;
            }
            var p1 = method1.GetParameters();
            var p2 = method2.GetParameters();
            if (p1.Length != p2.Length)
            {
                return false;
            }
            for (var i = 0; i < p1.Length; i++)
            {
                if (p1[i].ParameterType != p2[i].ParameterType)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
