using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace blqw
{
    /// <summary>
    /// 将日志输出到指定的 <seealso cref="TextWriter"/>
    /// </summary>
    class TextWriterLogger : ILogger
    {
        public TextWriterLogger(TextWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            _writer = writer.GetActualObject();
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            var str = GetString(state);
            _writer.WriteLine($"{GetIndent()}┏  {str}");
            var indent = _indent;
            Interlocked.Increment(ref _indent);
            return new EndScope(this, indent, str);
        }

        private void Unindent(int indent, string str)
        {
            Interlocked.CompareExchange(ref _indent, indent, indent + 1);
            _writer.WriteLine($"{GetIndent()}┗");
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (ReferenceEquals(state, ConsoleOutProxy.CONSOLE))
            {
                //过滤由控制台输出到ILogger的日志
                return;
            }
            var e = GetString(eventId);
            if (formatter != null)
            {
                _writer.WriteLine($"{GetIndent()}{GetString(logLevel)} : {formatter(state, exception)}{e}");
            }
            else
            {
                _writer.WriteLine($"{GetIndent()}{GetString(logLevel)} : {GetString(state, ref exception)}{e}");
                //循环输出异常
                while (exception != null)
                {
                    _writer.WriteLine(GetIndent() + exception.ToString());
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
        private readonly string[] _indentStrings = Enumerable.Range(0, 10).Select(x => string.Concat(Enumerable.Range(0, x).Select(y => "┃   "))).ToArray();

        private readonly TextWriter _writer;

        // 输入缩进
        string GetIndent()
        {
            var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ");
            var indent = _indent;
            return indent <= 0 ? time : time + (_indentStrings.ElementAtOrDefault(indent) ?? string.Concat(Enumerable.Range(0, indent).Select(y => "┃   ")));
        }

        // 获取日志等级的字符串
        private static string GetString(LogLevel logLevel)
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

        private static string GetString(object state)
        {
            switch (state)
            {
                case ILogFormattable a:
                    Exception ex = null;
                    return a.ToString(ref ex, null);
                case IFormattable b:
                    return b.ToString(null, null);
                case IConvertible c:
                    return c.ToString(null);
                default:
                    break;
            }
            return state.GetType() + " : " + state.ToString();
        }

        private static string GetString(object state, ref Exception exception)
        {
            switch (state)
            {
                case ILogFormattable a:
                    return a.ToString(ref exception, null);
                case IFormattable b:
                    return b.ToString(null, null);
                case IConvertible c:
                    return c.ToString(null);
                default:
                    break;
            }
            return $"{state.ToString()}({state.GetType()})";
        }
        // 获取事件的字符串
        private static string GetString(EventId eventId)
        {
            if (eventId.Id == 0)
            {
                return eventId.Name == null ? "" : " - " + eventId.Name;
            }
            else
            {
                return $" ({eventId.Name}:{eventId.Id})";
            }
        }

        // 取消缩进对象
        class EndScope : IDisposable
        {
            private TextWriterLogger _logger;
            private readonly string _str;

            public EndScope(TextWriterLogger consoleLogger, int indent, string str)
            {
                _logger = consoleLogger;
                Indent = indent;
                _str = str;
            }

            public int Indent { get; }

            // 取消缩进
            public void Dispose() => _logger.Unindent(Indent, _str);
        }
    }
}
