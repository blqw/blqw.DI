using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace blqw.DI
{
    /// <summary>
    /// 公开的扩展方法
    /// </summary>
    public static class StartupExtensions
    {
        #region LoadAllAssemblies

        readonly static ConcurrentDictionary<string, IList<Assembly>> _cache = new ConcurrentDictionary<string, IList<Assembly>>();

        /// <summary>
        /// 从指定路径载入所有程序集
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IList<Assembly> LoadAllAssemblies(this AppDomain domain, string path) =>
            _cache.GetOrAdd(path, p =>
            {
                var dm = AppDomain.CreateDomain("temp");

                foreach (var dll in Directory.GetFiles(p, "*.dll", SearchOption.AllDirectories))
                {
                    try
                    {
                        var ass = dm.Load(File.ReadAllBytes(dll));
                        domain.Load(ass.GetName());
                    }
                    catch (Exception) { }
                }

                AppDomain.Unload(dm);
                return domain.GetAssemblies().ToList().AsReadOnly();
            });

        /// <summary>
        /// 载入所有程序集
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IList<Assembly> LoadAllAssemblies(this AppDomain domain) =>
            domain.LoadAllAssemblies(domain.BaseDirectory);

        #endregion


        #region FindStartupTypes

        /// <summary>
        /// 根据 <seealso cref="AssemblyStartupAttribute"/> 特性查找启动类
        /// </summary>
        /// <param name="assembly"></param>
        public static IEnumerable<Type> FindStartupTypesByAttribute(this Assembly assembly)
            => assembly.FindTypes(Startup.Logger);

        /// <summary>
        /// 根据 <seealso cref="AssemblyStartupAttribute"/> 特性查找启动类
        /// </summary>
        /// <param name="assembly"></param>
        public static IEnumerable<Type> FindStartupTypesByAttribute(this IEnumerable<Assembly> assemblies)
        {
            List<Type> types;
            using (Startup.Logger.BeginScope("根据特性 [assembly: AssemblyStartup] 查找启动器类"))
            {
                types = assemblies.SelectMany(FindStartupTypesByAttribute).ToList();
            }
            Startup.Logger.Log($"启动器查找完成, 共 {types.Count} 个");
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

            var assName = assembly.GetName();
            List<Type> types;
            try
            {
                types = assembly.DefinedTypes.Where(x => x.Name == "Startup").ToList<Type>();
            }
            catch (ReflectionTypeLoadException ex)
            {
                Startup.Logger.Log(LogLevel.Warning, assembly, ex);
                types = ex.Types.Where(x => x.Name == "Startup").ToList();
            }
            if (types.Count > 0)
            {
                using (Startup.Logger.BeginScope($"程序集:{assName?.Name} {assName?.Version}"))
                {
                    foreach (var type in types)
                    {
                        Startup.Logger.Log(type.FullName);
                    }
                }
            }
            return types;
        }

        /// <summary>
        /// 查找 类型名称为 Startup 的启动类
        /// </summary>
        public static IEnumerable<Type> FindStartupTypesByName(this IEnumerable<Assembly> assemblies)
        {
            List<Type> types;
            using (Startup.Logger.BeginScope("根据名称 (Class.Name == \"Startup\") 查找启动器类"))
            {
                types = assemblies.SelectMany(FindStartupTypesByName).ToList();
                foreach (var type in types)
                {
                    Startup.Logger.Log(type.FullName);
                }
            }
            Startup.Logger.Log($"启动器查找完成, 共 {types.Count} 个");
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

            using (Startup.Logger.BeginScope("配置服务"))
            {
                startup.SetLoggerIfAbsent(Startup.Logger).ConfigureServices(services);
            }
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
        public static IServiceProvider Configure(this IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            var startups = serviceProvider.GetServices<IStartup>();
            if (startups == null)
            {
                return serviceProvider;
            }

            //获取日志服务
            var logger = serviceProvider.GetLogger<IStartup>();
            Startup.Logger = logger;
            using (logger.BeginScope("安装服务"))
            {
                foreach (var startup in startups)
                {
                    startup.SetLoggerIfAbsent(logger).Configure(serviceProvider);
                }
            }
            return serviceProvider;
        }
    }
}
