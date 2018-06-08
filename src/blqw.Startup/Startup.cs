using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace blqw
{
    /// <summary>
    /// 启动器
    /// </summary>
    public sealed class Startup
    {
        /// <summary>
        /// 用于标识 <seealso cref="IServiceCollection"/> 是否被处理过
        /// </summary>
        private static readonly ServiceDescriptor _serviceProcessed = new ServiceDescriptor(new { }.GetType(), p => null, ServiceLifetime.Transient);

        /// <summary>
        /// 寻找并执行启动器中的注册服务方法, 根据特性 <seealso cref="AssemblyStartupAttribute"/> 的标识寻找程序集中的启动器
        /// </summary>
        /// <param name="services">服务集合</param>
        public static void ConfigureServicesWithAttribute(IServiceCollection services)
        {
            ConfigureServicesCore(services, ass => ass.Select(x =>
            {
                var attr = x.GetCustomAttribute<AssemblyStartupAttribute>();
                if (attr == null)
                {
                    return null;
                }
                return x.GetType(attr.TypeFullName, false, false);
            }).Where(x => x != null));
        }

        /// <summary>
        /// 寻找并执行启动器中的注册服务方法, 根据特性 <seealso cref="AssemblyStartupAttribute"/> 的标识寻找程序集中的启动器, 返回服务集合
        /// </summary>
        /// <returns>服务集合</returns>
        public static IServiceCollection ConfigureServicesWithAttribute()
        {
            var services = new StartupServiceCollection();
            ConfigureServicesWithAttribute(services);
            return services;
        }

        private static void ConfigureServicesCore(IServiceCollection services, Func<IEnumerable<Assembly>, IEnumerable<Type>> selector)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            lock (services)
            {
                if (services.Contains(_serviceProcessed))
                {
                    throw new InvalidOperationException("无法重复调用");
                }
                services.Add(_serviceProcessed);
            }
            var configurable = new Configurable();
            using (configurable.Logger.BeginScope(services))
            {
                configurable.Logger.LogInformation("开始扫描启动器");
                var invokers = selector(AppDomain.CurrentDomain.GetAssemblies())
                                                 .Select(x => new StartupInvoker(x, configurable.Logger))
                                                 .Where(x => x.Type != null)
                                                 .ToList();
                configurable.SetInvoker(invokers);
                configurable.Logger.LogInformation($"启动器扫描完成, 共 {invokers.Count} 个");
                invokers.ForEach(x => x.ConfigureServices(services));
                services.AddSingleton(configurable);
            }
        }

        /// <summary>
        /// 寻找并执行启动器中的注册服务方法
        /// </summary>
        /// <param name="services">服务集合</param>
        [Obsolete("因为可能引起的性能问题和过早加载类型的问题而不建议使用, 建议使用 ConfigureServicesWithAttribute 方式")]
        public static void ConfigureServices(IServiceCollection services, Func<Type, bool> filter = null)
        {
            ConfigureServicesCore(services, ass => ass.SelectMany(x => x.Modules)
                                                      .SelectMany(x => x.GetTypes())
                                                      .Where(x => x.IsStatic())
                                                      .Where(x => x.Name == "Startup" && !ReferenceEquals(x, typeof(Startup)))
                                                      .Where(x => filter == null || filter(x)));
        }

        // 用于执行安装动作的服务
        class Configurable
        {
            private List<StartupInvoker> _invokers;

            public PrestoreLogger Logger { get; } = new PrestoreLogger();

            public void SetInvoker(List<StartupInvoker> invokers) => _invokers = invokers;

            public void Configure(IServiceProvider serviceProvider)
            {
                //获取日志服务
                var logger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger<Startup>()
                            ?? serviceProvider.GetService<ILogger>();
                //将预处理日志写入日志服务
                Logger.WriteTo(logger ?? new ConsoleLogger());
                //创建服务代理
                serviceProvider = new ServiceProviderProxy(serviceProvider);
                //循环安装服务
                _invokers.ForEach(x => x.Configure(serviceProvider));
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="filter"></param>
        public static void Configure(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            var configurable = serviceProvider.GetService<Configurable>();
            if (configurable == null)
            {
                throw new InvalidOperationException("未找到 Configurable 服务, 这可能是没有执行 ConfigureServices 或 ConfigureServicesWithAttribute");
            }
            using (configurable.Logger.BeginScope(serviceProvider))
            {
                configurable.Logger.LogInformation("开始安装服务");
                configurable.Configure(serviceProvider);
                configurable.Logger.LogInformation("服务安装完成");
            }
        }
    }
}
