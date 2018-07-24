using Microsoft.Extensions.Logging;
using System;
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

        public static T With<T>(this T obj,Action<T> action)
        {
            action?.Invoke(obj);
            return obj;
        }
    }
}
