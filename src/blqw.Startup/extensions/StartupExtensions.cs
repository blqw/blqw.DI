using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace blqw
{
    /// <summary>
    /// 公开的扩展方法
    /// </summary>
    public static class StartupExtensions
    {
        #region FindStartupTypes

        /// <summary>
        /// 根据 <seealso cref="AssemblyStartupAttribute"/> 特性查找启动类
        /// </summary>
        /// <param name="assembly"></param>
        public static IEnumerable<Type> FindStartupTypesByAttribute(this Assembly assembly)
            => AssemblyStartupAttribute.FindTypes(assembly, null);

        /// <summary>
        /// 根据 <seealso cref="AssemblyStartupAttribute"/> 特性查找启动类
        /// </summary>
        /// <param name="assembly"></param>
        public static IEnumerable<Type> FindStartupTypesByAttribute(this IEnumerable<Assembly> assemblies)
        {
            LogExtensions.Log(null, "根据特性 [assembly: AssemblyStartup] 查找启动器类");
            var types = assemblies.SelectMany(FindStartupTypesByAttribute).ToList();
            LogExtensions.Log(null, $"启动器查找完成, 共 {types.Count} 个");
            return types;
        }

        /// <summary>
        /// 根据 <seealso cref="AssemblyStartupAttribute"/> 特性查找启动类
        /// </summary>
        /// <param name="assembly"></param>
        public static IEnumerable<Type> FindStartupTypesByAttribute(this AppDomain domain)
            => FindStartupTypesByAttribute(domain?.GetAssemblies());

        /// <summary>
        /// 查找 类型名称为 Startup 的启动类
        /// </summary>
        /// <param name="assembly"></param>
        public static IEnumerable<Type> FindStartupTypesByName(this Assembly assembly)
        {
            if (assembly == null || assembly == Assembly.GetExecutingAssembly())
            {
                return Type.EmptyTypes;
            }
            try
            {
                return assembly.DefinedTypes.Where(x => x.Name == "Startup").ToList();
            }
            catch (ReflectionTypeLoadException ex)
            {
                LogExtensions.Log(null, LogLevel.Warning, assembly, ex);
                return ex.Types.Where(x => x.Name == "Startup").ToList();
            }
        }

        /// <summary>
        /// 查找 类型名称为 Startup 的启动类
        /// </summary>
        public static IEnumerable<Type> FindStartupTypesByName(this IEnumerable<Assembly> assemblies)
        {
            LogExtensions.Log(null, "根据名称 (Class.Name == \"Startup\") 查找启动器类 ");
            var types = assemblies.SelectMany(FindStartupTypesByName).ToList();
            LogExtensions.Log(null, $"启动器查找完成, 共 {types.Count} 个");
            return types;
        }

        /// <summary>
        /// 查找 类型名称为 Startup 的启动类
        /// </summary>
        public static IEnumerable<Type> FindStartupTypesByName(this AppDomain domain)
            => FindStartupTypesByName(domain?.GetAssemblies());


        #endregion


        #region ConfigureServices

        /// <summary>
        /// 调用启动器的配置服务并注册启动器
        /// </summary>
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IStartup startup)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (startup == null)
            {
                return services;
            }

            startup.ConfigureServices(services);
            services.AddSingleton(startup);
            return services;
        }

        /// <summary>
        /// 调用启动类中的 ConfigureServices 方法来配置服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="startupTypes">待调用 ConfigureServices 的启动类</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IEnumerable<Type> startupTypes) =>
            ConfigureServices(services, AggregateStartup.Create(startupTypes));

        /// <summary>
        /// 根据 <seealso cref="AssemblyStartupAttribute"/> 特性查找启动类, 并调用启动类中的 ConfigureServices 方法来配置服务
        /// </summary>
        public static IServiceCollection ConfigureServices(this IServiceCollection services) =>
            ConfigureServices(services, AppDomain.CurrentDomain.FindStartupTypesByAttribute());

        #endregion

        /// <summary>
        /// 安装服务
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void Configure(this IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            var startups = serviceProvider.GetServices<IStartup>();
            if (startups == null)
            {
                return;
            }

            //获取日志服务
            var logger = serviceProvider.GetLogger();

            using (logger.BeginScope(typeof(Startup)))
            {
                logger.Info("开始安装服务");
                foreach (var startup in startups)
                {
                    startup.SetLoggerIfAbsent(logger).Configure(serviceProvider);
                }
                logger.Info("服务安装完成");
            }
        }
    }
}
