using System;
using System.Collections.Generic;
using System.Text;

namespace blqw
{
    /// <summary>
    /// 代理接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface IProxy<T>
    {
        T ActualTarget { get; }
    }
}
