using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demo2
{
    class MyLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => new MyLogger();
        public void Dispose() { }
    }

    class MyLogger : ILogger
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter != null)
            {
                Console.WriteLine($"{logLevel} {eventId} "  + formatter(state, exception));
            }
            else
            {
                Console.WriteLine($"{logLevel} {eventId} {state} {exception}");
            }
        }

        public bool IsEnabled(LogLevel logLevel) => true;
        public IDisposable BeginScope<TState>(TState state)
        {
            Console.WriteLine($"Begin {state}");
            return new ScopeEnd(state);
        }

        class ScopeEnd:IDisposable
        {
            private object _state;

            public ScopeEnd(object state) => this._state = state;

            public void Dispose() => Console.WriteLine($"End {_state}");
        }
    }
}
