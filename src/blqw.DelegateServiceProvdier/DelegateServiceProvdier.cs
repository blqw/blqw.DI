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
        readonly IServiceProvider _provider;
        readonly ILogger _logger;
        readonly ConcurrentDictionary<Type, object> _services;

        /// <summary>
        /// 构造一个服务提供程序的代理
        /// </summary>
        /// <param name="provider">被代理的服务提供程序</param>
        public DelegateServiceProvdier(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = _provider.GetService<ILoggerFactory>()?.CreateLogger<DelegateServiceProvdier>();
            _services = new ConcurrentDictionary<Type, object>(TypeComparer.Instance);
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


            if (service == null)
            {
                // 当常规方式没有获取到服务，且服务是委托类型时，尝试获取MethodInfo服务，并返回最后一个签名相同的MethodInfo并转换为指定类型的委托
                return typeof(Delegate).IsAssignableFrom(serviceType)
                        ? _services.GetOrAdd(serviceType, x => ConvertDelegate(x, _provider.GetServices<MethodInfo>()))
                        : null;
            }

            if (service is Delegate delegateService)
            {
                // 当获取的服务是委托，但与要求的类型不符时，尝试转换委托类型
                if (serviceType is IServiceProvider tp
                    && tp.GetService(typeof(Type)) is Type delegateType
                    && typeof(Delegate).IsAssignableFrom(delegateType)
                    && !delegateType.IsInstanceOfType(service))
                {
                    return _services.GetOrAdd(serviceType, x => ConvertDelegate(delegateType, new[] { delegateService.Method }));
                }
                return service;
            }

            if (service is IEnumerable enumerable && serviceType.IsGenericType && serviceType.GetGenericArguments().Length == 1)
            {
                // 当获取的服务是泛型集合时
                var type = serviceType.GetGenericArguments()[0];
                if (type is IServiceProvider tp && tp.GetService(typeof(Type)) is Type delegateType)
                {
                    return _services.GetOrAdd(serviceType, x => ConvertDelegates(delegateType, enumerable));
                }
                else
                {
                    return _services.GetOrAdd(serviceType, x => ConvertDelegates(type, _provider.GetServices<MethodInfo>()));
                }
            }
            return service;
        }

        /// <summary>
        /// 转换委托服务集合
        /// </summary>
        /// <param name="delegateType"></param>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        private IEnumerable ConvertDelegates(Type delegateType, IEnumerable enumerable)
        {
            var newServices = new ArrayList();
            var delegateMethod = delegateType.GetMethod("Invoke");
            foreach (var item in enumerable)
            {
                if (delegateType.IsInstanceOfType(item))
                {
                    newServices.Add(item);
                    continue;
                }
                var method = (item as Delegate)?.Method ?? item as MethodInfo;
                if (CompareMethodSignature(delegateMethod, method))
                {
                    newServices.Add(method.CreateDelegate(delegateType, null));
                }
            }
            return newServices.ToArray(delegateType);
        }

        /// <summary>
        /// 转换委托服务
        /// </summary>
        private object ConvertDelegate(Type delegateType, IEnumerable<MethodInfo> methods)
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
