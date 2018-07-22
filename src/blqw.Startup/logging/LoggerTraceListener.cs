using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace blqw
{
    class LoggerTraceListener : TraceListener
    {
        public LoggerTraceListener(ILogger logger) => Logger = logger;

        public ILogger Logger { get; }

        public override void Write(string message) => Logger.Log(LogLevel.Trace, 0, message, null, null);
        public override void WriteLine(string message) => Logger.Log(LogLevel.Trace, 0, message, null, null);
    }
}
