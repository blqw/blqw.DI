using Microsoft.Extensions.DependencyInjection;
using System;

namespace blqw
{
    /// <summary>
    /// 启动器接口
    /// </summary>
    public interface IStartup
    {
        /// <summary>
        /// 配置服务
        /// </summary>
        /// <param name="services"></param>
        void ConfigureServices(IServiceCollection services);

        /// <summary>
        /// 安装服务
        /// </summary>
        /// <param name="serviceProvider"></param>
        void Configure(IServiceProvider serviceProvider);
    }
}
