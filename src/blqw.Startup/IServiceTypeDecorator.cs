using System;
using System.Collections.Generic;
using System.Text;

namespace blqw
{
    /// <summary>
    /// 服务类型装饰器
    /// </summary>
    public interface IServiceTypeDecorator
    {
        Type ServiceType { get; }
    }
}
