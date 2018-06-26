using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace blqw
{
    class MulticastLogger : ILogger
    {
        private readonly ILogger[] _loggers;

        public MulticastLogger(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            _loggers = serviceProvider.GetServices<ILogger>().ToArray();
        }

        class EndScope : IDisposable
        {
            private readonly IDisposable[] _disposables;

            public EndScope(IEnumerable<IDisposable> disposables) => _disposables = disposables.ToArray();

            public void Dispose() => Array.ForEach(_disposables, x => x.Dispose());
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state) => new EndScope(_loggers.Select(x => x.BeginScope(state)));


        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            foreach (var logger in _loggers.Where(x => x.IsEnabled(logLevel)))
            {
                logger.Log(logLevel, eventId, state, exception, formatter);
            }
        }
    }
}
