using System;
using System.Reflection;

namespace blqw
{
    /// <summary>
    /// 指定当前程序集的启动类
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
    public sealed class AssemblyStartupAttribute : Attribute
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
    }
}
