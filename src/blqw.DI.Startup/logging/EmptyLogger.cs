using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace blqw.DI.logging
{
    class EmptyLogger : ILogger, IDisposable
    {
        public static readonly EmptyLogger Instance = new EmptyLogger();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
        }
        public bool IsEnabled(LogLevel logLevel) => false;
        public IDisposable BeginScope<TState>(TState state) => this;
        public void Dispose() { }
    }
}
