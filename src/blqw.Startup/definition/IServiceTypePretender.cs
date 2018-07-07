using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;

namespace blqw
{
    /// <summary>
    /// 服务类型伪装
    /// </summary>
    public interface IServiceTypePretender
    {
        /// <summary>
        /// 真实服务类型
        /// </summary>
        Type ServiceType { get; }

    }
}
