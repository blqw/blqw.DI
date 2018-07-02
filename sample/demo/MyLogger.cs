using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace demo
{
    class MyLogger : ILogger
    {
        private readonly string _file;

        public MyLogger(string file)
        {
            if (!File.Exists(file))
            {
                File.Create(file);
            }

            _file = file;
        }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter == null)
            {
                File.AppendAllText(_file, new { logLevel, eventId, state, exception }.ToString() + Environment.NewLine);
            }
            else if (state is Type t && t == typeof(Console))
            {
                File.AppendAllText(_file, formatter(state, exception) + Environment.NewLine);
            }
            else
            {
                File.AppendAllText(_file, new { logLevel, eventId, formatter = formatter(state, exception) }.ToString() + Environment.NewLine);
            }
        }

        public bool IsEnabled(LogLevel logLevel) => true;
        public IDisposable BeginScope<TState>(TState state)
        {
            File.AppendAllText(_file, "BeginScope :" + state.ToString() + Environment.NewLine);
            return new EndScope(() => File.AppendAllText(_file, "EndScope :" + state.ToString() + Environment.NewLine));
        }

        class EndScope : IDisposable
        {
            private readonly Action _dispose;

            public EndScope(Action dispose) => _dispose = dispose;

            public object State { get; }

            public void Dispose() => _dispose();
        }
    }
}
