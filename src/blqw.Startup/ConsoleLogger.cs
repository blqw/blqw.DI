using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace blqw
{
    /// <summary>
    /// 控制台日志
    /// </summary>
    class ConsoleLogger : ILogger
    {
        private int _indent = 0;
        private readonly string[] _indents = new[] {
            "",
            new string(' ', 4),
            new string(' ', 4*2),
            new string(' ', 4*3),
            new string(' ', 4*4),
            new string(' ', 4*5),
            new string(' ', 4*6),
        };

        private readonly string[] _levels = new[] { "Trace", "Debug", "Info ", "Warn ", "Error", "Criti", "None " };

        private string GetString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace: return "Trace";
                case LogLevel.Debug: return "Debug";
                case LogLevel.Information: return "Info ";
                case LogLevel.Warning: return "Warn ";
                case LogLevel.Error: return "Error";
                case LogLevel.Critical: return "Criti";
                case LogLevel.None: return "None ";
                default: return logLevel.ToString();
            }
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var e = GetEventString(eventId);
            if (formatter != null)
            {
                WriteIndent();
                Console.WriteLine($"{GetString(logLevel)}{e} : {formatter(state, exception)}");
            }
            else
            {
                WriteIndent();
                Console.WriteLine($"{GetString(logLevel)}{e} : {state.ToString()}");
                //循环输出异常
                while (exception != null)
                {
                    WriteIndent();
                    Console.WriteLine(exception.ToString());
                    // 获取基础异常
                    var ex = exception.GetBaseException();
                    // 基础异常获取失败则获取 内部异常
                    if (ex == null || ex == exception)
                    {
                        // 预防出现一些极端例子导致死循环
                        if (ex == exception.InnerException)
                        {
                            return;
                        }
                        ex = exception.InnerException;
                    }
                }
            }
        }

        private static string GetEventString(EventId eventId)
        {
            if (eventId.Id == 0)
            {
                if (eventId.Name == null)
                {
                    return "";
                }
                else
                {
                    return " - " + eventId.Name;
                }
            }
            else if (eventId.Name == null)
            {
                return $" - [{eventId.Id}]";
            }
            else
            {
                return $" - [{eventId.Id}]{eventId.Name}";
            }
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        void WriteIndent()
        {
            var indent = _indent;
            if (indent > 0)
            {
                if (indent < _indents.Length)
                {
                    Console.Write(_indents[indent]);
                }
                else
                {
                    Console.Write(new string(' ', indent * 4));
                }
            }
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            WriteIndent();
            Console.WriteLine(typeof(TState).FullName + ":" + state?.ToString());
            Interlocked.Increment(ref _indent);
            return new Unindent(this);
        }

        class Unindent : IDisposable
        {
            private ConsoleLogger consoleLogger;

            public Unindent(ConsoleLogger consoleLogger) => this.consoleLogger = consoleLogger;

            public void Dispose() => Interlocked.Decrement(ref consoleLogger._indent);
        }
    }
}
