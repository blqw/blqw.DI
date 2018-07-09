using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace blqw
{
    /// <summary>
    /// 指定当前程序集的启动类
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
    public sealed class AssemblyStartupAttribute : Attribute, ILoggable, ILogFormattable
    {
        /// <summary>
        /// 指定当前程序集的启动类
        /// </summary>
        /// <param name="typeFullName">启动类全名</param>
        /// <exception cref="ArgumentNullException"><paramref name="typeFullName"/>为null, 空字符串或连续空白</exception>
        public AssemblyStartupAttribute(string typeFullName)
        {
            if (string.IsNullOrWhiteSpace(typeFullName))
            {
                throw new ArgumentNullException(nameof(typeFullName));
            }

            TypeFullName = typeFullName;
        }

        /// <summary>
        /// 指定当前程序集的启动类
        /// </summary>
        /// <param name="typeFullName">启动类全名</param>
        /// <exception cref="ArgumentNullException"><paramref name="typeFullName"/>为null, 空字符串或连续空白</exception>
        public AssemblyStartupAttribute(Type type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            TypeFullName = type.FullName;
        }

        /// <summary>
        /// 启动类全名
        /// </summary>
        public string TypeFullName { get; }

        /// <summary>
        /// 启动类
        /// </summary>
        public Type Type { get; }


        private Type GetType(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var type = Type;
            if (type == null)
            {
                type = assembly.GetType(TypeFullName, false, false);
                if (type == null)
                {
                    Logger.Warn($"FindStartupTypes异常:{TypeFullName} 类型没有找到");
                }
                return type;
            }
            else if (type.Assembly != assembly)
            {
                Logger.Warn($"FindStartupTypes异常:{type} 类型因为跨程序引用集而被忽略");
                return null;
            }
            else
            {
                return type;
            }
        }

        /// <summary>
        /// 查找程序集中被 <see cref="AssemblyStartupAttribute"/> 标识的启动类
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IEnumerable<Type> FindTypes(Assembly assembly, ILogger logger)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            try
            {
                var attrs = assembly.GetCustomAttributes<AssemblyStartupAttribute>();
                if (attrs == null || attrs.Any() == false)
                {
                    return Type.EmptyTypes;
                }
                var assName = assembly.GetName();
                using ((logger ?? ConsoleLogger.Instance).BeginScope($"程序集:{assName?.Name} {assName?.Version}"))
                {
                    var types = attrs.Select(x => x.SetLogger(logger).GetType(assembly)).Where(t => t != null).ToList();
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

        /// <summary>
        /// 获取或设置日志组件
        /// </summary>
        public ILogger Logger { get; set; }


        string ILogFormattable.ToString(ref Exception exception, IFormatProvider formatProvider) =>
            "AssemblyStartup:" + (TypeFullName ?? Type?.FullName ?? "未知");
    }
}
