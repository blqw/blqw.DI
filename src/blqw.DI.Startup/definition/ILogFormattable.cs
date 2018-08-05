using System;

namespace blqw
{

    /// <summary>
    /// 可格式化自定义日志信息
    /// </summary>
    public interface ILogFormattable
    {
        /// <summary>
        /// 返回格式化后的字符串
        /// </summary>
        /// <param name="exception">日志中的异常信息, 可在方法中直接修改该对象, 如果不希望执行输出操作, 则可将该值改为null</param>
        /// <param name="formatProvider"></param>
        string ToString(ref Exception exception, IFormatProvider formatProvider);
    }
}
