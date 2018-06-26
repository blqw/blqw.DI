using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace blqw
{
    /// <summary>
    /// 公开的扩展方法
    /// </summary>
    public static class StartupExtensions
    {
        public static void Configure(this IServiceProvider serviceProvider) =>
            Startup.Configure(serviceProvider);

        public static IServiceCollection ConfigureServicesWithAttribute(this IServiceCollection services) =>
            Startup.ConfigureServicesWithAttribute(services);

        public static IServiceCollection AddServers(this IServiceCollection services, Action<IServiceCollection> configureServices)
        {
            configureServices(services);
            return services;
        }

        public static IServiceCollection AddConsoleLog(this IServiceCollection services)
        {
            services.AddSingleton<ILogger>(new TextWriterLogger(Console.Out));
            return services;
        }

        public static ILogger GetLogger(this IServiceProvider serviceProvider) =>
            new MulticastLogger(serviceProvider);

        public static ILogger GetLogger(this IServiceProvider serviceProvider, string categoryName) =>
            serviceProvider.GetService<ILoggerFactory>()?.CreateLogger(categoryName) ??
            serviceProvider.GetService<ILoggerProvider>()?.CreateLogger(categoryName);

        public static ILogger GetLogger<T>(this IServiceProvider serviceProvider) =>
            serviceProvider.GetService<ILoggerFactory>()?.CreateLogger<T>() ??
            serviceProvider.GetService<ILoggerProvider>()?.CreateLogger(typeof(T).FullName);

        public static IServiceProvider ConsoleToLogger(this IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetLogger();
            if (logger != null)
            {
                logger.ConsoleToLogger();
            }
            return serviceProvider;
        }

        public static void ConsoleToLogger(this ILogger logger)
        {
            if (!(Console.Out is MulticastTextWriter))
            {
                Console.SetOut(new MulticastTextWriter(Console.Out, Console.Out));
            }
        }
    }
}
