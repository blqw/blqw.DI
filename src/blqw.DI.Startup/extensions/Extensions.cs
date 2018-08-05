using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace blqw
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    static class Extensions
    {
        /// <summary>
        /// 判断类型是否是静态类
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsStatic(this Type type) => type != null && type.IsAbstract && type.IsSealed;

        /// <summary>
        /// 尝试从服务器提供程序中获取方法参数的值
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static object GetParameterValue(this IServiceProvider serviceProvider, MethodInfo method, ParameterInfo parameter)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (parameter == null)
            {
                return null;
            }

            if (parameter.ParameterType.IsInstanceOfType(serviceProvider))
            {
                return serviceProvider;
            }

            return serviceProvider.GetService(parameter.ParameterType) ?? (parameter.HasDefaultValue ? parameter.DefaultValue : null);
        }

        /// <summary>
        /// 获取代理对象(如果存在)的真实对象
        /// </summary>
        public static T GetActualObject<T>(this T obj) =>
            obj is IProxy<T> t ? t.ActualTarget : obj;

        public static T With<T>(this T obj, Action<T> action)
        {
            action?.Invoke(obj);
            return obj;
        }

        /// <summary>
        /// 查找程序集中被 AssemblyStartupAttribute 标识的启动类
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IEnumerable<Type> FindTypes(this Assembly assembly, ILogger logger)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            try
            {
                var attrs = assembly.GetCustomAttributes().Where(x => x.GetType().Name == "AssemblyStartupAttribute").ToArray();
                if (attrs.Length == 0)
                {
                    return Type.EmptyTypes;
                }
                var assName = assembly.GetName();
                using (Startup.Logger.BeginScope($"程序集:{assName?.Name} {assName?.Version}"))
                {
                    var types = attrs.Select(x => x.GetType(assembly, logger)).Where(t => t != null).ToList();
                    foreach (var type in types)
                    {
                        logger.Log($"启动器 -> {type.FullName}");
                    }
                    return types;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, assembly);
                return Type.EmptyTypes;
            }
        }

        static T GetPropertyValue<T>(this object obj, string propertyName)
            where T : class
        {
            var prop = obj?.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (prop == null)
            {
                return default(T);
            }
            if (prop.GetGetMethod().IsStatic)
            {
                return prop.GetValue(null) as T;
            }
            return prop.GetValue(obj) as T;
        }

        public static Type GetType(this Attribute startupAttribute, Assembly assembly, ILogger logger)
        {
            if (startupAttribute == null)
            {
                throw new ArgumentNullException(nameof(startupAttribute));
            }

            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }


            var type = startupAttribute.GetPropertyValue<Type>("Type");
            if (type != null)
            {
                if (type.Assembly != assembly)
                {
                    logger.Warn($"FindStartupTypes异常:{type} 类型因为跨程序引用集而被忽略");
                    return null;
                }
            }
            return type;
        }
    }
}
