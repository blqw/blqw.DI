using Microsoft.Extensions.Logging;
using System;

namespace blqw.Logging
{
    /// <summary>
    /// 控制台日志
    /// </summary>
    class ConsoleLogger : TextWriterLogger
    {
        public static ILoggerProvider LoggerProvider = new Provider();
        class Provider : ILoggerProvider
        {
            public ILogger CreateLogger(string categoryName) => new ConsoleLogger(categoryName);
            public void Dispose() { }
        }

        public ConsoleLogger(string categoryName)
            : base(Console.Out, categoryName)
        {
        }

        public override void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            //过滤由控制台转发到ILogger的日志
            if (ReferenceEquals(state, typeof(Console)))
            {
                return;
            }
            base.Log(logLevel, eventId, state, exception, formatter);
        }

        public override void Dispose() { } //控制台日志不能释放
    }
}
