using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;

namespace blqw
{
    /// <summary>
    /// 控制台日志
    /// </summary>
    class ConsoleLogger : TextWriterLogger
    {
        public static ConsoleLogger Instance { get; } = new ConsoleLogger();

        private ConsoleLogger()
            : base(Console.Out)
        {
        }

        public override void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            //过滤由控制台转发到ILogger的日志
            if (ConsoleForwarder.IsForwarding(state))
            {
                return;
            }
            base.Log(logLevel, eventId, state, exception, formatter);
        }

        public override void Dispose() { } //控制台日志不能释放
    }
}
