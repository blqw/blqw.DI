using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace blqw
{
    /// <summary>
    /// 启动器总线
    /// </summary>
    public sealed class Startup
    {
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
