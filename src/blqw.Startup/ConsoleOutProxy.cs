using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace blqw
{
    sealed class ConsoleOutProxy : TextWriter, ILogger, IProxy<TextWriter>
    {
        public static readonly Type CONSOLE = typeof(Console);

        private ILogger _logger;

        public ConsoleOutProxy(ILogger logger)
        {
            _logger = logger.GetActualObject();
            BaseWriter = Console.Out.GetActualObject();
        }

        TextWriter IProxy<TextWriter>.ActualTarget => BaseWriter;


        #region TextWriter
        public override Encoding Encoding => BaseWriter?.Encoding;

        public override IFormatProvider FormatProvider => BaseWriter?.FormatProvider;

        public override string NewLine
        {
            get => BaseWriter?.NewLine;
            set
            {
                if (BaseWriter != null)
                {
                    BaseWriter.NewLine = value;
                }
            }
        }

        public TextWriter BaseWriter { get; private set; }

        public override void Close()
        {
            base.Close();
            BaseWriter.Close();
            _logger = null;
            BaseWriter = null;
        }

        public override void Flush() => BaseWriter?.Flush();

        public override Task FlushAsync() => BaseWriter?.FlushAsync() ?? Task.CompletedTask;

        private TextWriter WriteCore(char value)
        {
            if (char.IsWhiteSpace(value))
            {
                return BaseWriter;
            }
            if (_logger?.IsEnabled(LogLevel.Trace) == true)
            {
                _logger.Log(LogLevel.Trace, 0, CONSOLE, null, (a, b) => value.ToString(FormatProvider));
            }
            return BaseWriter;
        }

        private TextWriter WriteCore(char[] buffer, int index, int count)
        {
            if (buffer == null || buffer.Length == 0 || count <= 0 || index >= buffer.Length)
            {
                return BaseWriter;
            }
            if (_logger?.IsEnabled(LogLevel.Trace) == true)
            {
                if (index < 0)
                {
                    index = 0;
                }
                if (count > buffer.Length - index)
                {
                    count = buffer.Length - index;
                }
                if (count == 0)
                {
                    return BaseWriter;
                }
                else if (count == 1)
                {
                    return WriteCore(buffer[index]);
                }
                return WriteCore(new string(buffer, index, count));
            }
            return BaseWriter;
        }

        private TextWriter WriteCore(object value)
        {
            if (value == null)
            {
                return BaseWriter;
            }
            if (value is string s)
            {
                return WriteCore(s);
            }
            if (_logger?.IsEnabled(LogLevel.Trace) == true)
            {
                switch (value)
                {
                    case Exception ex:
                        _logger.Log(LogLevel.Trace, 0, CONSOLE, ex, (a, b) => (ex.GetBaseException() ?? ex)?.ToString());
                        break;
                    case IFormattable f:
                        return WriteCore(f.ToString(null, FormatProvider));
                    default:
                        WriteLine(value.ToString());
                        break;
                }

            }
            return BaseWriter;
        }

        private TextWriter WriteCore(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return BaseWriter;
            }
            if (_logger?.IsEnabled(LogLevel.Trace) == true)
            {
                _logger.Log(LogLevel.Trace, 0, CONSOLE, null, (a, b) => value ?? string.Empty);
            }
            return BaseWriter;
        }


        public override void Write(char value) => WriteCore(value)?.Write(value);

        public override void Write(char[] buffer, int index, int count) => WriteCore(buffer, index, count)?.Write(buffer, index, count);

        public override void Write(object value) => WriteCore(value)?.Write(value);

        public override void Write(string value) => WriteCore(value)?.Write(value);

        public override Task WriteAsync(char value) => WriteCore(value)?.WriteAsync(value);

        public override Task WriteAsync(char[] buffer, int index, int count) => WriteCore(buffer, index, count)?.WriteAsync(buffer, index, count);

        public override Task WriteAsync(string value) => WriteCore(value)?.WriteAsync(value);

        public override void WriteLine() => BaseWriter?.WriteLine();

        public override void WriteLine(char value) => WriteCore(value)?.Write(value);

        public override void WriteLine(char[] buffer, int index, int count) => WriteCore(buffer, index, count)?.WriteLine(buffer, index, count);

        public override void WriteLine(object value) => WriteCore(value)?.WriteLine(value);
        public override void WriteLine(string value) => WriteCore(value)?.WriteLine(value);

        public override Task WriteLineAsync() => BaseWriter?.WriteLineAsync();

        public override Task WriteLineAsync(char value) => WriteCore(value)?.WriteLineAsync(value);

        public override Task WriteLineAsync(char[] buffer, int index, int count) => WriteCore(buffer, index, count)?.WriteLineAsync(buffer, index, count);

        public override Task WriteLineAsync(string value) => WriteCore(value)?.WriteLineAsync(value);
        #endregion

        #region ILogger

        public IDisposable BeginScope<TState>(TState state)
        {
            WriteIndent();
            BaseWriter.WriteLine(typeof(TState).FullName + ":" + state?.ToString());
            var indent = _indent;
            Interlocked.Increment(ref _indent);
            return new EndScope(this, indent);
        }

        private void Unindent(int indent) =>
            Interlocked.CompareExchange(ref _indent, indent, indent + 1);

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (ReferenceEquals(state, CONSOLE))
            {
                //过滤由控制台输出到ILogger的日志
                return;
            }
            var e = GetEventString(eventId);
            if (formatter != null)
            {
                WriteIndent();
                BaseWriter.WriteLine($"{GetString(logLevel)}{e} : {formatter(state, exception)}");
            }
            else
            {
                WriteIndent();
                if (state is ILogFormattable formattable)
                {
                    BaseWriter.WriteLine($"{GetString(logLevel)}{e} : {formattable.ToString(ref exception, null)}");
                }
                else
                {
                    BaseWriter.WriteLine($"{GetString(logLevel)}{e} : {state.ToString()}");
                }
                //循环输出异常
                while (exception != null)
                {
                    WriteIndent();
                    BaseWriter.WriteLine(exception.ToString());
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
            BaseWriter.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff "));
            var indent = _indent;
            if (indent > 0)
            {
                BaseWriter.Write(_indentStrings.ElementAtOrDefault(indent) ?? new string(' ', indent * 4));
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
        class EndScope : IDisposable
        {
            private ConsoleOutProxy _consoleLogger;

            public EndScope(ConsoleOutProxy consoleLogger, int indent)
            {
                _consoleLogger = consoleLogger;
                Indent = indent;
            }

            public int Indent { get; }

            // 取消缩进
            public void Dispose() => _consoleLogger.Unindent(Indent);
        }

        #endregion
    }
}
