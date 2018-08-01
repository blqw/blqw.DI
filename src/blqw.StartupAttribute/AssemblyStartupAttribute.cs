using System;


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
        /// <param name="type">启动类</param>
        public AssemblyStartupAttribute(Type type) => Type = type;

        /// <summary>
        /// 启动类
        /// </summary>
        public Type Type { get; }
    }
}