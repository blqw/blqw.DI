using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace blqw
{
    class LoggerTraceListener : TraceListener
    {
        public LoggerTraceListener(ILogger logger) => Logger = logger;

        public ILogger Logger { get; }

        public override void Write(string message) => WriteLine(message);
        public override void WriteLine(string message) => Logger.Log<string>(LogLevel.Trace, 0, message, null, null);
    }
}
