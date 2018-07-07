using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace blqw
{
    /// <summary>
    /// 控制台日志
    /// </summary>
    class ConsoleLogger : TextWriterLogger
    {
        public static ConsoleLogger Instance { get; } = new ConsoleLogger();

        private ConsoleLogger()
            : base(Console.Out)
        {
        }
    }
}
