﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace blqw.DI
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// 注入字段和属性
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="instance"> 待注入的对象实例或类型实例 </param>
        /// <returns></returns>
        public static IServiceProvider Autowrite(this IServiceProvider serviceProvider, object instance)
        {
            if (serviceProvider == null || instance == null)
            {
                return serviceProvider;
            }
            var flags = BindingFlags.Public | BindingFlags.NonPublic;
            var type = instance as Type ?? instance.GetType();
            if (instance is Type)
            {
                instance = null;
                flags |= BindingFlags.Static;
            }
            else
            {
                flags |= BindingFlags.Instance;
            }


            foreach (var field in type.GetFields(flags))
            {
                var attr = field.GetCustomAttributes().OfType<IServiceProviderFactory<IServiceProvider>>().LastOrDefault();
                var value = attr?.CreateServiceProvider(serviceProvider).GetServiceOrCreateInstance(field.FieldType);
                if (value != null)
                {
                    field.SetValue(instance, value);
                }
            }

            foreach (var property in type.GetProperties(flags))
            {
                var attr = property.GetCustomAttributes().OfType<IServiceProviderFactory<IServiceProvider>>().FirstOrDefault();
                var value = attr?.CreateServiceProvider(serviceProvider).GetServiceOrCreateInstance(property.PropertyType);
                if (value != null)
                {
                    property.Set(instance, value);
                }
            }

            return serviceProvider;
        }

        /// <summary>
        /// 从服务中获取对象或动态创建对象, 并注入字段和属性
        /// </summary>
        /// <param name="serviceProvider">服务提供程序</param>
        /// <param name="type">待获取或创建的对象类型</param>
        /// <returns></returns>
        public static object GetServiceOrCreateInstance(this IServiceProvider serviceProvider, Type type)
        {
            if (serviceProvider == null)
            {
                return Activator.CreateInstance(type);
            }

            var obj = ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, type);
            if (obj != null)
            {
                serviceProvider.Autowrite(obj);
            }
            return obj;
        }

        /// <summary>
        /// 获取用于创建指定对象并注入字段和属性的委托方法
        /// </summary>
        /// <param name="serviceProvider">服务提供程序</param>
        /// <param name="instanceType">待创建的对象类型</param>
        /// <param name="argumentTypes">额外构造参数</param>
        /// <returns></returns>
        public static ObjectFactory CreateFactory(this IServiceProvider serviceProvider, Type instanceType, Type[] argumentTypes)
        {
            var factory = ActivatorUtilities.CreateFactory(instanceType, argumentTypes);
            if (factory == null)
            {
                return factory;
            }
            return (provider, args) =>
            {
                var obj = factory(provider, args);
                if (obj != null)
                {
                    provider?.Autowrite(obj);
                }
                return obj;
            };
        }

        /// <summary>
        /// 动态创建对象, 并注入字段和属性
        /// </summary>
        /// <param name="provider">服务提供程序</param>
        /// <param name="instanceType">待创建的对象类型</param>
        /// <param name="parameters">额外构造参数</param>
        /// <returns></returns>
        public static object CreateInstance(this IServiceProvider provider, Type instanceType, params object[] parameters)
        {
            var obj = ActivatorUtilities.CreateInstance(provider, instanceType, parameters);
            if (obj != null)
            {
                provider.Autowrite(obj);
            }
            return obj;
        }

        /// <summary>
        /// 动态创建对象, 并注入字段和属性
        /// </summary>
        /// <typeparam name="T">待创建的对象类型</typeparam>
        /// <param name="provider">服务提供程序</param>
        /// <param name="parameters">额外构造参数</param>
        /// <returns></returns>
        public static T CreateInstance<T>(this IServiceProvider provider, params object[] parameters) =>
            (T)CreateInstance(provider, typeof(T), parameters);

        /// <summary>
        /// 从服务中获取对象或动态创建对象, 并注入字段和属性
        /// </summary>
        /// <typeparam name="T">待获取或创建的对象类型</typeparam>
        /// <param name="provider">服务提供程序</param>
        /// <returns></returns>
        public static T GetServiceOrCreateInstance<T>(this IServiceProvider provider) =>
            (T)GetServiceOrCreateInstance(provider, typeof(T));

        /// <summary>
        /// 通过服务中的 <see cref="IServiceProviderFactory{IServiceProvider}"/> 重新编译提供程序
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IServiceProvider RebuildFromFactory(this IServiceProvider provider)
        {
            var factories = provider.GetServices<IServiceProviderFactory<IServiceProvider>>();
            foreach (var factory in factories)
            {
                provider = factory.CreateServiceProvider(provider);
            }
            return provider;
        }
    }
}
