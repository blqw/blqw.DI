using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    sealed class LoggerTextWriter : TextWriter
    {
        public static readonly Type STATE = typeof(Console);

        private ILogger _logger;

        public LoggerTextWriter(ILogger logger, TextWriter baseWriter)
        {
            _logger = logger;
            BaseWriter = baseWriter;
        }

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

        private TextWriter Log(char value)
        {
            if (char.IsWhiteSpace(value))
            {
                return BaseWriter;
            }
            if (_logger?.IsEnabled(LogLevel.Trace) == true)
            {
                _logger.Log(LogLevel.Trace, 0, STATE, null, (a, b) => value.ToString(FormatProvider));
            }
            return BaseWriter;
        }

        private TextWriter Log(char[] buffer, int index, int count)
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
                    return Log(buffer[index]);
                }
                return Log(new string(buffer, index, count));
            }
            return BaseWriter;
        }

        private TextWriter Log(object value)
        {
            if (value == null)
            {
                return BaseWriter;
            }
            if (value is string s)
            {
                return Log(s);
            }
            if (_logger?.IsEnabled(LogLevel.Trace) == true)
            {
                switch (value)
                {
                    case Exception ex:
                        _logger.Log(LogLevel.Trace, 0, STATE, ex, (a, b) => (ex.GetBaseException() ?? ex)?.ToString());
                        break;
                    case IFormattable f:
                        return Log(f.ToString(null, FormatProvider));
                    default:
                        WriteLine(value.ToString());
                        break;
                }

            }
            return BaseWriter;
        }

        private TextWriter Log(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return BaseWriter;
            }
            if (_logger?.IsEnabled(LogLevel.Trace) == true)
            {
                _logger.Log(LogLevel.Trace, 0, STATE, null, (a, b) => value ?? string.Empty);
            }
            return BaseWriter;
        }


        public override void Write(char value) => Log(value)?.Write(value);

        public override void Write(char[] buffer, int index, int count) => Log(buffer, index, count)?.Write(buffer, index, count);

        public override void Write(object value) => Log(value)?.Write(value);

        public override void Write(string value) => Log(value)?.Write(value);

        public override Task WriteAsync(char value) => Log(value)?.WriteAsync(value);

        public override Task WriteAsync(char[] buffer, int index, int count) => Log(buffer, index, count)?.WriteAsync(buffer, index, count);

        public override Task WriteAsync(string value) => Log(value)?.WriteAsync(value);

        public override void WriteLine() => BaseWriter?.WriteLine();

        public override void WriteLine(char value) => Log(value)?.Write(value);

        public override void WriteLine(char[] buffer, int index, int count) => Log(buffer, index, count)?.WriteLine(buffer, index, count);

        public override void WriteLine(object value) => Log(value)?.WriteLine(value);
        public override void WriteLine(string value) => Log(value)?.WriteLine(value);

        public override Task WriteLineAsync() => BaseWriter?.WriteLineAsync();

        public override Task WriteLineAsync(char value) => Log(value)?.WriteLineAsync(value);

        public override Task WriteLineAsync(char[] buffer, int index, int count) => Log(buffer, index, count)?.WriteLineAsync(buffer, index, count);

        public override Task WriteLineAsync(string value) => Log(value)?.WriteLineAsync(value);
    }
}
