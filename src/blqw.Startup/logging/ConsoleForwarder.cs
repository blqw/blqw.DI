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
    /// <summary>
    /// 控制台内容转发器
    /// </summary>
    sealed class ConsoleForwarder : TextWriter, IProxy<TextWriter>
    {
        private static readonly Type LOG_STATE = typeof(Console);

        public static bool IsForwarding(object state) => ReferenceEquals(state, LOG_STATE);

        private ILogger _logger;

        public ConsoleForwarder(ILogger logger)
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
                _logger.Log(LogLevel.Trace, 0, LOG_STATE, null, (a, b) => value.ToString(FormatProvider));
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
                        _logger.Log(LogLevel.Trace, 0, LOG_STATE, ex, (a, b) => (ex.GetBaseException() ?? ex)?.ToString());
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
                _logger.Log(LogLevel.Trace, 0, LOG_STATE, null, (a, b) => value ?? string.Empty);
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
    }
}
