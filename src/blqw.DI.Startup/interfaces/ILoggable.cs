using Microsoft.Extensions.Logging;

namespace blqw.DI
{
    /// <summary>
    /// 可记录日志的
    /// </summary>
    internal interface ILoggable
    {
        /// <summary>
        /// 日志记录
        /// </summary>
        ILogger Logger { get; set; }
    }
}
