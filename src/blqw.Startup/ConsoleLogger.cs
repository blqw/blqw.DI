using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;

namespace blqw
{
    /// <summary>
    /// 控制台日志, 用于将日志输出到控制台
    /// </summary>
    class ConsoleLogger : ILogger
    {

        public IDisposable BeginScope<TState>(TState state)
        {
            WriteIndent();
            Console.WriteLine(typeof(TState).FullName + ":" + state?.ToString());
            Interlocked.Increment(ref _indent);
            return new Unindent(this);
        }

        public bool IsEnabled(LogLevel logLevel) => true;

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

        // 当前缩进
        private int _indent = 0;
        // 生成1~10个空格字符串的缩进
        private readonly string[] _indentStrings = Enumerable.Range(0, 10).Select(x => new string(' ', x * 4)).ToArray();

        // 输入缩进
        void WriteIndent()
        {
            var indent = _indent;
            if (indent > 0)
            {
                Console.Write(_indentStrings.ElementAtOrDefault(indent) ?? new string(' ', indent * 4));
            }
        }

        // 获取日志等级的字符串
        private string GetString(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace: return "【Trace】";
                case LogLevel.Debug: return "【Debug】";
                case LogLevel.Information: return "【Info 】";
                case LogLevel.Warning: return "【Warn 】";
                case LogLevel.Error: return "【Error】";
                case LogLevel.Critical: return "【Criti】";
                case LogLevel.None: return "【None 】";
                default: return logLevel.ToString();
            }
        }

        // 获取事件的字符串
        private static string GetEventString(EventId eventId)
        {
            if (eventId.Id == 0)
            {
                return eventId.Name == null ? "" : " - " + eventId.Name;
            }
            else
            {
                return $" - [{eventId.Id}]{eventId.Name}";
            }
        }

        // 取消缩进对象
        class Unindent : IDisposable
        {
            private ConsoleLogger _consoleLogger;

            public Unindent(ConsoleLogger consoleLogger) => _consoleLogger = consoleLogger;
            // 取消缩进
            public void Dispose() => Interlocked.Decrement(ref _consoleLogger._indent);
        }
    }
}
