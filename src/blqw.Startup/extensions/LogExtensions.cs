using blqw;
using blqw.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// 日志相关扩展方法
    /// </summary>
    public static class LogExtensions
    {
        private static string GetEventName(string path, string member, int line = 0)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return member?.Trim() + (line == 0 ? "" : ":" + line);
            }
            if (string.IsNullOrWhiteSpace(member))
            {
                return path?.Trim() + (line == 0 ? "" : ":" + line);
            }
            return Path.GetFileNameWithoutExtension(path.Trim()) + "." + member.Trim() + (line == 0 ? "" : ":" + line);
        }

        private static ILogger DefaultLogger { get; } = new ConsoleLogger(null);

        /// <summary>
        /// 标准日志输出
        /// </summary>
        public static void Log(this ILogger logger, LogLevel logLevel, object messageOrObject,
                                Exception exception = null,
                                [CallerMemberName] string memberName = "",
                                [CallerFilePath] string sourceFilePath = "",
                                [CallerLineNumber] int sourceLineNumber = 0)
                => (logger ?? DefaultLogger).Log(logLevel, new EventId(sourceLineNumber, GetEventName(sourceFilePath, memberName)), messageOrObject, exception, null);

        /// <summary>
        /// 错误日志输出
        /// </summary>
        public static void Error(this ILogger logger, Exception exception,
                                object messageOrObject = null,
                                [CallerMemberName] string memberName = "",
                                [CallerFilePath] string sourceFilePath = "",
                                [CallerLineNumber] int sourceLineNumber = 0)
                => (logger ?? DefaultLogger).Log(LogLevel.Error, new EventId(sourceLineNumber, GetEventName(sourceFilePath, memberName)), messageOrObject, exception, null);

        /// <summary>
        /// 严重错误日志输出
        /// </summary>
        public static void Critical(this ILogger logger, Exception exception,
                                object messageOrObject = null,
                                [CallerMemberName] string memberName = "",
                                [CallerFilePath] string sourceFilePath = "",
                                [CallerLineNumber] int sourceLineNumber = 0)
                => (logger ?? DefaultLogger).Log(LogLevel.Critical, new EventId(sourceLineNumber, GetEventName(sourceFilePath, memberName)), messageOrObject, exception, null);

        /// <summary>
        /// 标准日志输出
        /// </summary>
        public static void Log(this ILogger logger, object messageOrObject,
                                Exception exception = null,
                                [CallerMemberName] string memberName = "",
                                [CallerFilePath] string sourceFilePath = "",
                                [CallerLineNumber] int sourceLineNumber = 0)
                => (logger ?? DefaultLogger).Log(LogLevel.Trace, new EventId(sourceLineNumber, GetEventName(sourceFilePath, memberName)), messageOrObject, exception, null);

        /// <summary>
        /// 调试日志输出
        /// </summary>
        public static void Debug(this ILogger logger, object messageOrObject,
                                Exception exception = null,
                                [CallerMemberName] string memberName = "",
                                [CallerFilePath] string sourceFilePath = "",
                                [CallerLineNumber] int sourceLineNumber = 0)
                => (logger ?? DefaultLogger).Log(LogLevel.Debug, new EventId(sourceLineNumber, GetEventName(sourceFilePath, memberName)), messageOrObject, exception, null);

        /// <summary>
        /// 普通信息日志输出
        /// </summary>
        public static void Info(this ILogger logger, object messageOrObject,
                                Exception exception = null,
                                [CallerMemberName] string memberName = "",
                                [CallerFilePath] string sourceFilePath = "",
                                [CallerLineNumber] int sourceLineNumber = 0)
                => (logger ?? DefaultLogger).Log(LogLevel.Information, new EventId(sourceLineNumber, GetEventName(sourceFilePath, memberName)), messageOrObject, exception, null);

        /// <summary>
        /// 警告日志输出
        /// </summary>
        public static void Warn(this ILogger logger, object messageOrObject,
                                Exception exception = null,
                                [CallerMemberName] string memberName = "",
                                [CallerFilePath] string sourceFilePath = "",
                                [CallerLineNumber] int sourceLineNumber = 0)
                => (logger ?? DefaultLogger).Log(LogLevel.Warning, new EventId(sourceLineNumber, GetEventName(sourceFilePath, memberName)), messageOrObject, exception, null);

        /// <summary>
        /// 如果目标实例为<seealso cref="ILoggable"/>> ,且 <seealso cref="ILoggable.Logger"/>  属性为空, 则设置为指定的日志组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loggable"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        internal static T SetLoggerIfAbsent<T>(this T instance, ILogger logger)
            => SetLogger(instance, logger, false);

        /// <summary>
        /// 设置目标实例为<seealso cref="ILoggable"/>> ,则设置 <seealso cref="ILoggable.Logger"/> 属性为指定的日志组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loggable"></param>
        /// <param name="logger"></param>
        /// <param name="replace">如果参数为true则无条件替换, 否则当目标实例Logger属性不为空时不执行任何操作</param>
        /// <returns></returns>
        internal static T SetLogger<T>(this T instance, ILogger logger, bool replace = true)
        {
            if (instance is ILoggable loggable && loggable.Logger == null)
            {
                loggable.Logger = logger;
            }
            return instance;
        }

        /// <summary>
        /// 将通过 <see cref="Trace"/> 记录的内容转发到日志
        /// </summary>
        public static IServiceProvider TraceForwardingToLogger(this IServiceProvider serviceProvider)
        {
            var factory = serviceProvider?.GetService<ILoggerFactory>();
            if (factory != null)
            {
                var categoryName = TypeNameHelper.GetTypeDisplayName(typeof(Console));
                var logger = factory.CreateLogger(categoryName);
                WatchTrace(logger);
            }
            return serviceProvider;
        }

        public static IServiceProvider AddConsoleLogger(this IServiceProvider serviceProvider) =>
            serviceProvider.With(x => x.GetRequiredService<ILoggerFactory>().AddProvider(ConsoleLogger.LoggerProvider));

        ///// <summary>
        ///// 控制台内容转发到日志
        ///// </summary>
        //public static IServiceProvider ConsoleForwardingToLogger(this IServiceProvider serviceProvider)
        //{
        //    var factory = serviceProvider?.GetService<ILoggerFactory>();
        //    if (factory != null)
        //    {
        //        var categoryName = TypeNameHelper.GetTypeDisplayName(typeof(Console));
        //        var logger = factory.CreateLogger(categoryName);
        //        WatchConsole(logger);
        //    }
        //    return serviceProvider;
        //}

        /// <summary>
        /// 获取日志服务
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static ILogger GetLogger(this IServiceProvider serviceProvider, string categoryName)
        {
            var factory = serviceProvider?.GetService<ILoggerFactory>();
            if (factory != null)
            {
                return factory.CreateLogger(categoryName);
            }
            //如果不存在任何服务, 则返回默认服务
            return new ConsoleLogger(categoryName);
        }

        /// <summary>
        /// 获取日志服务
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static ILogger GetLogger(this IServiceProvider serviceProvider, Type type) =>
            GetLogger(serviceProvider, TypeNameHelper.GetTypeDisplayName(type));

        /// <summary>
        /// 获取日志服务
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static ILogger GetLogger<T>(this IServiceProvider serviceProvider) =>
            serviceProvider.GetLogger(TypeNameHelper.GetTypeDisplayName(typeof(T)));

        public static IServiceProvider AddLogging(this IServiceProvider serviceProvider, Action<ILoggerFactory> configure)
        {
            var factory = serviceProvider?.GetService<ILoggerFactory>();
            if (factory != null)
            {
                configure(factory);
            }
            return serviceProvider;
        }

        /// <summary>
        /// 观察控制台输出，并转发到指定的日志组件
        /// </summary>
        /// <param name="logger"></param>
        [Obsolete("废弃", true)]
        private static void WatchConsole(this ILogger logger) =>
             Console.SetOut(new ConsoleForwarder(logger));

        /// <summary>
        /// 观察控制台输出，并转发到指定的日志组件
        /// </summary>
        /// <param name="logger"></param>
        private static void WatchTrace(this ILogger logger)
        {
            if (Trace.Listeners.OfType<LoggerTraceListener>().Any(x => x.Logger == logger))
            {
                return;
            }
            Trace.Listeners.Add(new LoggerTraceListener(logger));
        }
    }
}
