using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace blqw.Logging
{
    /// <summary>
    /// 将日志输出到指定的 <seealso cref="TextWriter"/>
    /// </summary>
    public class TextWriterLogger : ILogger, IDisposable
    {
        public TextWriterLogger(TextWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            Writer = writer.GetActualObject();
        }

        public TextWriterLogger(TextWriter writer, string categoryName) : this(writer) => this.CategoryName = categoryName;

        protected void ThrowIfDisposed()
        {
            if (Writer == null)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        protected const LogLevel SCOPE_BEGIN = (LogLevel)(-1);
        protected const LogLevel SCOPE_END = (LogLevel)(int.MinValue);

        public virtual bool IsEnabled(LogLevel logLevel) => LogLevel <= logLevel;

        public virtual LogLevel LogLevel { get; protected set; } = 0;

        public virtual void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            ThrowIfDisposed();
            var e = GetString(eventId);
            if (formatter != null)
            {
                Writer.WriteLine($"{Time} {GetString(logLevel)}{GetIndent()} {formatter(state, exception)}{e}");
            }
            else
            {
                Writer.WriteLine($"{Time} {GetString(logLevel)}{GetIndent()} {GetString(state, ref exception)}{e}");
                //循环输出异常
                while (exception != null)
                {
                    Writer.WriteLine($"{Time} {GetIndent()}{exception.ToString()}");
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
                    exception = ex;
                }
            }
            Writer.Flush();
        }

        public virtual IDisposable BeginScope<TState>(TState state)
        {
            ThrowIfDisposed();
            var str = GetString(state);
            Writer.WriteLine($"{Time} {GetString(SCOPE_BEGIN)}{GetIndent()}┏  {str}");
            var indent = _indent;
            Interlocked.Increment(ref _indent);
            Writer.Flush();
            return new ScopeEnds(this, indent, str);
        }

        private void Unindent(int indent, string str)
        {
            Interlocked.CompareExchange(ref _indent, indent, indent + 1);
            Writer?.WriteLine($"{Time} {GetString(SCOPE_END)}{GetIndent()}┗");
            Writer?.Flush();
        }

        // 当前缩进
        private int _indent = 0;

        // 生成1~10个空格字符串的缩进
        private readonly string[] _indentStrings = Enumerable.Range(0, 10).Select(x => string.Concat(Enumerable.Range(0, x).Select(y => "┃   "))).ToArray();

        private string Time => GetString(DateTime.Now);

        protected TextWriter Writer { get; private set; }

        public string CategoryName { get; }

        /// <summary>
        /// 获取缩进字符串
        /// </summary>
        /// <returns></returns>
        protected virtual string GetIndent()
        {
            var indent = _indent;
            return indent <= 0 ? "" : _indentStrings.ElementAtOrDefault(indent) ?? string.Concat(Enumerable.Range(0, indent).Select(y => "┃   "));
        }

        /// <summary>
        /// 获取时间对应的字符串
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        protected virtual string GetString(DateTime time) => time.ToString("yyyy-MM-dd HH:mm:ss.fff");

        /// <summary>
        /// 获取日志等级的字符串
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        protected virtual string GetString(LogLevel logLevel)
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
                case SCOPE_BEGIN: return "【Begin】";
                case SCOPE_END: return "【 End 】";
                default: return logLevel.ToString();
            }
        }

        private string GetString(object state)
        {
            Exception _ = null;
            return GetString(state, ref _);
        }
        /// <summary>
        /// 获得 <see cref="state" /> 对象的字符串
        /// </summary>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        protected virtual string GetString(object state, ref Exception exception)
        {
            switch (state)
            {
                case ILogFormattable a:
                    return a.ToString(ref exception, null);
                case IFormattable b:
                    return b.ToString(null, null);
                case IConvertible c:
                    return c.ToString(null);
                case null:
                    return "<null>";
                default:
                    return $"{state.ToString()}({state.GetType()})";
            }
        }
        /// <summary>
        /// 获取事件的字符串
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        protected virtual string GetString(EventId eventId)
        {
            if (eventId.Id == 0)
            {
                return eventId.Name ?? CategoryName ?? "";
            }
            else
            {
                return $" ({eventId.Name ?? CategoryName ?? "<unknown>"}:{eventId.Id})";
            }
        }

        // 取消缩进对象
        class ScopeEnds : IDisposable
        {
            private TextWriterLogger _logger;
            private readonly string _str;

            public ScopeEnds(TextWriterLogger consoleLogger, int indent, string str)
            {
                _logger = consoleLogger;
                Indent = indent;
                _str = str;
            }

            public int Indent { get; }

            // 取消缩进
            public void Dispose() => _logger.Unindent(Indent, _str);
        }

        /// <summary>
        /// 释放 <see cref="TextWriter"/>
        /// </summary>
        public virtual void Dispose()
        {
            var writer = Writer;
            Writer = null;
            if (writer != null)
            {
                writer.Dispose();
            }
        }
    }
}
