using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace blqw.DI
{
    /// <summary>
    /// 启动器总线
    /// </summary>
    public sealed class Startup
    {
        private static ILogger _logger = new PrestoreLogger();

        internal static ILogger Logger
        {
            get => _logger;
            set
            {
                if (_logger is PrestoreLogger pre)
                {
                    pre.WriteTo(value);  //将预存日志写入新日志组件
                    _logger = value;
                }
            }
        }

        /// <summary>
        /// 创建一个 <seealso cref="IServiceCollection"/>
        /// </summary>
        public static IServiceCollection CreateServiceCollection() =>
            new ServiceCollectionDecorator(new List<ServiceDescriptor>());

        /// <summary>
        /// 根据 <seealso cref="AssemblyStartupAttribute"/> 特性查找启动类, 并调用启动类中的 ConfigureServices 方法来配置服务，返回服务集合
        /// </summary>
        /// <returns>返回服务集合</returns>
        public static IServiceCollection ConfigureServices() =>
            CreateServiceCollection().ConfigureServices();

        /// <summary>
        /// 安装服务
        /// </summary>
        /// <param name="serviceProvider">服务提供程序</param>
        public static void Configure(IServiceProvider serviceProvider) =>
            serviceProvider.Configure();
    }
}
