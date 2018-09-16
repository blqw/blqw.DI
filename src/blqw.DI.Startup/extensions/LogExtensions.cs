using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace blqw.DI
{
    /// <summary>
    /// 日志相关扩展方法
    /// </summary>
    internal static class LogExtensions
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

        /// <summary>
        /// 标准日志输出
        /// </summary>
        public static void Log(this ILogger logger, LogLevel logLevel, object messageOrObject,
                                Exception exception = null,
                                [CallerMemberName] string memberName = "",
                                [CallerFilePath] string sourceFilePath = "",
                                [CallerLineNumber] int sourceLineNumber = 0)
                => logger?.Log(logLevel, new EventId(sourceLineNumber, GetEventName(sourceFilePath, memberName)), messageOrObject, exception, null);

        /// <summary>
        /// 错误日志输出
        /// </summary>
        public static void Error(this ILogger logger, Exception exception,
                                object messageOrObject = null,
                                [CallerMemberName] string memberName = "",
                                [CallerFilePath] string sourceFilePath = "",
                                [CallerLineNumber] int sourceLineNumber = 0)
                => logger?.Log(LogLevel.Error, new EventId(sourceLineNumber, GetEventName(sourceFilePath, memberName)), messageOrObject, exception, null);

        /// <summary>
        /// 严重错误日志输出
        /// </summary>
        public static void Critical(this ILogger logger, Exception exception,
                                object messageOrObject = null,
                                [CallerMemberName] string memberName = "",
                                [CallerFilePath] string sourceFilePath = "",
                                [CallerLineNumber] int sourceLineNumber = 0)
                => logger?.Log(LogLevel.Critical, new EventId(sourceLineNumber, GetEventName(sourceFilePath, memberName)), messageOrObject, exception, null);

        /// <summary>
        /// 标准日志输出
        /// </summary>
        public static void Log(this ILogger logger, object messageOrObject,
                                Exception exception = null,
                                [CallerMemberName] string memberName = "",
                                [CallerFilePath] string sourceFilePath = "",
                                [CallerLineNumber] int sourceLineNumber = 0)
                => logger?.Log(LogLevel.Trace, new EventId(sourceLineNumber, GetEventName(sourceFilePath, memberName)), messageOrObject, exception, null);

        /// <summary>
        /// 调试日志输出
        /// </summary>
        public static void Debug(this ILogger logger, object messageOrObject,
                                Exception exception = null,
                                [CallerMemberName] string memberName = "",
                                [CallerFilePath] string sourceFilePath = "",
                                [CallerLineNumber] int sourceLineNumber = 0)
                => logger?.Log(LogLevel.Debug, new EventId(sourceLineNumber, GetEventName(sourceFilePath, memberName)), messageOrObject, exception, null);

        /// <summary>
        /// 普通信息日志输出
        /// </summary>
        public static void Info(this ILogger logger, object messageOrObject,
                                Exception exception = null,
                                [CallerMemberName] string memberName = "",
                                [CallerFilePath] string sourceFilePath = "",
                                [CallerLineNumber] int sourceLineNumber = 0)
                => logger?.Log(LogLevel.Information, new EventId(sourceLineNumber, GetEventName(sourceFilePath, memberName)), messageOrObject, exception, null);

        /// <summary>
        /// 警告日志输出
        /// </summary>
        public static void Warn(this ILogger logger, object messageOrObject,
                                Exception exception = null,
                                [CallerMemberName] string memberName = "",
                                [CallerFilePath] string sourceFilePath = "",
                                [CallerLineNumber] int sourceLineNumber = 0)
                => logger?.Log(LogLevel.Warning, new EventId(sourceLineNumber, GetEventName(sourceFilePath, memberName)), messageOrObject, exception, null);

        /// <summary>
        /// 如果目标实例为<seealso cref="ILoggable"/>> ,且 <seealso cref="ILoggable.Logger"/>  属性为空, 则设置为指定的日志组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static T SetLoggerIfAbsent<T>(this T instance, ILogger logger)
            => SetLogger(instance, logger, false);

        /// <summary>
        /// 设置目标实例为<seealso cref="ILoggable"/>> ,则设置 <seealso cref="ILoggable.Logger"/> 属性为指定的日志组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
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
        /// 获取日志服务
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static ILogger GetLogger<T>(this IServiceProvider serviceProvider) =>
                serviceProvider?.GetService<ILogger<T>>() ??
                serviceProvider?.GetService<ILoggerFactory>()?.CreateLogger(TypeNameHelper.GetTypeDisplayName(typeof(T)));
    }
}
