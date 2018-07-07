using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace blqw
{
    /// <summary>
    /// 日志相关扩展方法
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// 标准日志输出
        /// </summary>
        public static void Log(this ILogger logger, LogLevel logLevel, object messageOrObject,
                                Exception exception = null,
                                [CallerMemberName] string memberName = "",
                                [CallerFilePath] string sourceFilePath = "",
                                [CallerLineNumber] int sourceLineNumber = 0)
                => (logger ?? ConsoleLogger.Instance).Log(logLevel, new EventId(sourceLineNumber, memberName), messageOrObject, exception, null);

        /// <summary>
        /// 错误日志输出
        /// </summary>
        public static void Error(this ILogger logger, Exception exception,
                                object messageOrObject = null,
                                [CallerMemberName] string memberName = "",
                                [CallerFilePath] string sourceFilePath = "",
                                [CallerLineNumber] int sourceLineNumber = 0)
                => (logger ?? ConsoleLogger.Instance).Log(LogLevel.Error, new EventId(sourceLineNumber, memberName), messageOrObject, exception, null);

        /// <summary>
        /// 严重错误日志输出
        /// </summary>
        public static void Critical(this ILogger logger, Exception exception,
                                object messageOrObject = null,
                                [CallerMemberName] string memberName = "",
                                [CallerFilePath] string sourceFilePath = "",
                                [CallerLineNumber] int sourceLineNumber = 0)
                => (logger ?? ConsoleLogger.Instance).Log(LogLevel.Critical, new EventId(sourceLineNumber, memberName), messageOrObject, exception, null);

        /// <summary>
        /// 标准日志输出
        /// </summary>
        public static void Log(this ILogger logger, object messageOrObject,
                                Exception exception = null,
                                [CallerMemberName] string memberName = "",
                                [CallerFilePath] string sourceFilePath = "",
                                [CallerLineNumber] int sourceLineNumber = 0)
                => (logger ?? ConsoleLogger.Instance).Log(LogLevel.Trace, new EventId(sourceLineNumber, memberName), messageOrObject, exception, null);

        /// <summary>
        /// 调试日志输出
        /// </summary>
        public static void Debug(this ILogger logger, object messageOrObject,
                                Exception exception = null,
                                [CallerMemberName] string memberName = "",
                                [CallerFilePath] string sourceFilePath = "",
                                [CallerLineNumber] int sourceLineNumber = 0)
                => (logger ?? ConsoleLogger.Instance).Log(LogLevel.Debug, new EventId(sourceLineNumber, memberName), messageOrObject, exception, null);

        /// <summary>
        /// 普通信息日志输出
        /// </summary>
        public static void Info(this ILogger logger, object messageOrObject,
                                Exception exception = null,
                                [CallerMemberName] string memberName = "",
                                [CallerFilePath] string sourceFilePath = "",
                                [CallerLineNumber] int sourceLineNumber = 0)
                => (logger ?? ConsoleLogger.Instance).Log(LogLevel.Information, new EventId(sourceLineNumber, memberName), messageOrObject, exception, null);

        /// <summary>
        /// 警告日志输出
        /// </summary>
        public static void Warn(this ILogger logger, object messageOrObject,
                                Exception exception = null,
                                [CallerMemberName] string memberName = "",
                                [CallerFilePath] string sourceFilePath = "",
                                [CallerLineNumber] int sourceLineNumber = 0)
                => (logger ?? ConsoleLogger.Instance).Log(LogLevel.Warning, new EventId(sourceLineNumber, memberName), messageOrObject, exception, null);

        /// <summary>
        /// 如果目标实例中 Logger 属性为空, 则设置为指定的日志组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loggable"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static T SetLoggerIfAbsent<T>(this T instance, ILogger logger)
            => SetLogger(instance, logger, false);

        /// <summary>
        /// 设置目标实例中 Logger 属性为指定的日志组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loggable"></param>
        /// <param name="logger"></param>
        /// <param name="replace">如果参数为true则无条件替换, 否则当目标实例Logger属性不为空时不执行任何操作</param>
        /// <returns></returns>
        public static T SetLogger<T>(this T instance, ILogger logger, bool replace = true)
        {
            if (instance is ILoggable loggable && loggable.Logger == null)
            {
                loggable.Logger = logger;
            }
            return instance;
        }

        /// <summary>
        /// 添加控制台日志服务
        /// </summary>
        public static IServiceCollection AddConsoleLogger(this IServiceCollection services)
        {
            if (services.Any(x => x.ImplementationInstance == ConsoleLogger.Instance))
            {
                return services;
            }
            return services.AddSingleton(ConsoleLogger.Instance);
        }

        /// <summary>
        /// 控制台内容转发到日志
        /// </summary>
        public static IServiceProvider ConsoleForwardingToLogger(this IServiceProvider serviceProvider)
        {
            var logger = serviceProvider?.GetLogger();
            if (logger != null)
            {
                WatchConsole(logger);
            }
            return serviceProvider;
        }

        /// <summary>
        /// 控制台内容转发到日志
        /// </summary>
        public static IServiceCollection ConsoleForwardingToLogger(this IServiceCollection services) =>
            services?.AddSingleton<IStartup>(new ActionStartup(null, provider => ConsoleForwardingToLogger(provider)));

        /// <summary>
        /// 获取日志服务
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static ILogger GetLogger(this IServiceProvider serviceProvider)
        {
            var loggers = serviceProvider.GetServices<ILogger>();
            if (!loggers.Any())
            {
                //如果不存在任何服务, 则返回默认服务
                loggers = new[] { ConsoleLogger.Instance };
            }
            return new AggregateLogger(loggers);
        }


        /// <summary>
        /// 观察控制台输出，并转发到指定的日志组件
        /// </summary>
        /// <param name="logger"></param>
        public static void WatchConsole(this ILogger logger)
        {
            if (Console.Out is ConsoleOutProxy == false)
            {
                Console.SetOut(new ConsoleOutProxy(logger));
            }
        }
    }
}
