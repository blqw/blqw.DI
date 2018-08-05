using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace blqw
{
    /// <summary>
    /// 基于委托构造的安装器
    /// </summary>
    class ActionStartup : IStartup, ILoggable
    {
        public ActionStartup(Action<IServiceCollection> configureServices, Action<IServiceProvider> configure)
        {
            ConfigureServices = configureServices;
            Configure = configure;
        }

        public ILogger Logger { get; set; }
        public Action<IServiceCollection> ConfigureServices { get; }
        public Action<IServiceProvider> Configure { get; }

        void IStartup.ConfigureServices(IServiceCollection services)
        {
            try
            {
                ConfigureServices?.Invoke(services);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ConfigureServices?.Method?.Name);
            }
        }
        void IStartup.Configure(IServiceProvider serviceProvider)
        {
            try
            {
                Configure?.Invoke(serviceProvider);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, Configure?.Method?.Name);
            }
        }
    }
}
