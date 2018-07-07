using Microsoft.Extensions.Logging;

namespace blqw
{
    /// <summary>
    /// 可记录日志的
    /// </summary>
    public interface ILoggable
    {
        /// <summary>
        /// 日志记录
        /// </summary>
        ILogger Logger { get; set; }
    }
}
