using System;
using System.Collections.Generic;
using System.Text;

namespace blqw.DI
{
    /// <summary>
    /// 用于创建支持上下文的 <see cref="IServiceProvider"/>
    /// </summary>
    public static class ServiceContextFactory
    {
        /// <summary>
        /// 创建一个支持上下文的 <see cref="IServiceProvider"/>
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IServiceProvider Create(IServiceProvider provider) =>
            provider as SupportContextServiceProvider ?? new SupportContextServiceProvider(provider, null);
    }
}
