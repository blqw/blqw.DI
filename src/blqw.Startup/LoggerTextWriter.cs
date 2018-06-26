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

        public LoggerTextWriter(ILogger logger, Encoding encoding, IFormatProvider formatProvider)
        {
            _logger = logger;
            Encoding = encoding;
            FormatProvider = formatProvider;
        }

        public override Encoding Encoding { get; }

        public override IFormatProvider FormatProvider { get; }

        public override void Close()
        {
            base.Close();
            _logger = null;
        }

        public override void Flush() { }
        public override Task FlushAsync() => Task.CompletedTask;

        public override void Write(char value) => WriteLine(value);
        public override void Write(char[] buffer, int index, int count) => WriteLine(buffer, index, count);
        public override void Write(object value) => WriteLine(value);
        public override void Write(string value) => WriteLine(value);

        public override Task WriteAsync(char value)
        {
            WriteLine(value);
            return Task.CompletedTask;
        }

        public override Task WriteAsync(char[] buffer, int index, int count)
        {
            WriteLine(buffer, index, count);
            return Task.CompletedTask;
        }

        public override Task WriteAsync(string value)
        {
            WriteLine(value);
            return Task.CompletedTask;
        }

        public override void WriteLine()
        {
            if (_logger?.IsEnabled(LogLevel.Trace) == true)
            {
                _logger.Log(LogLevel.Trace, 0, STATE, null, (a, b) => string.Empty);
            }
        }

        public override void WriteLine(char value)
        {
            if (_logger?.IsEnabled(LogLevel.Trace) == true)
            {
                _logger.Log(LogLevel.Trace, 0, STATE, null, (a, b) => value.ToString(FormatProvider));
            }
        }
        public override void WriteLine(char[] buffer, int index, int count)
        {
            if (_logger?.IsEnabled(LogLevel.Trace) == true)
            {
                if (buffer == null)
                {
                    _logger.Log(LogLevel.Trace, 0, STATE, null, (a, b) => string.Empty);
                    return;
                }
                if (index < 0)
                {
                    index = 0;
                }
                if (count > buffer.Length - index)
                {
                    count = buffer.Length - index;
                }
                _logger.Log(LogLevel.Trace, 0, STATE, null, (a, b) => new string(buffer, index, count));
            }
        }
        public override void WriteLine(object value)
        {
            if (_logger?.IsEnabled(LogLevel.Trace) == true)
            {
                switch (value)
                {
                    case null:
                        _logger.Log(LogLevel.Trace, 0, STATE, null, (a, b) => string.Empty);
                        break;
                    case Exception ex:
                        _logger.Log(LogLevel.Trace, 0, STATE, ex, (a, b) => (ex.GetBaseException() ?? ex)?.ToString());
                        break;
                    case IFormattable f:
                        _logger.Log(LogLevel.Trace, 0, STATE, null, (a, b) => f.ToString(null, FormatProvider));
                        break;
                    default:
                        WriteLine(value.ToString());
                        break;
                }

            }
        }
        public override void WriteLine(string value)
        {
            if (_logger?.IsEnabled(LogLevel.Trace) == true)
            {
                _logger.Log(LogLevel.Trace, 0, STATE, null, (a, b) => value ?? string.Empty);
            }
        }

        public override Task WriteLineAsync()
        {
            WriteLine();
            return Task.CompletedTask;
        }

        public override Task WriteLineAsync(char value)
        {
            WriteLine(value);
            return Task.CompletedTask;
        }

        public override Task WriteLineAsync(char[] buffer, int index, int count)
        {
            WriteLine(buffer, index, count);
            return Task.CompletedTask;
        }

        public override Task WriteLineAsync(string value)
        {
            WriteLine(value);
            return Task.CompletedTask;
        }
    }
}
