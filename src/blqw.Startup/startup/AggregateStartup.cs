using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace blqw
{
    /// <summary>
    /// 服务聚合启动器
    /// </summary>
    internal sealed class AggregateStartup : IStartup, ILoggable
    {
        /// <summary>
        /// 启动器代理
        /// </summary>
        private List<IStartup> _startups;
        private ILogger _logger;


        public ILogger Logger
        {
            get => _logger;
            set
            {
                if (_logger is PrestoreLogger pre)
                {
                    pre.WriteTo(value);  //将预存日志写入新日志组件
                }
                _logger = value;
            }
        }

        /// <summary>
        /// 创建聚合启动器
        /// </summary>
        /// <param name="startupTypes"></param>
        /// <returns></returns>
        public static AggregateStartup Create(IEnumerable<Type> startupTypes)
        {
            if (startupTypes == null)
            {
                return null;
            }
            var startups = startupTypes.Where(x => x != null).Distinct().Select(x => new StartupProxy(x));
            return new AggregateStartup(startups);
        }


        /// <summary>
        /// 初始化服务聚合启动器
        /// </summary>
        /// <param name="startupTypes">启动类</param>
        public AggregateStartup(IEnumerable<IStartup> startups)
        {
            if (startups == null)
            {
                throw new ArgumentNullException(nameof(startups));
            }
            Logger = Startup.Logger;
            _startups = startups.ToList();
            foreach (var startup in _startups)
            {
                if (startup is ILoggable loggable && loggable.Logger == null)
                {
                    loggable.Logger = Logger;
                }
            }
        }

        /// <summary>
        /// 从启动类配置服务
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            _startups.ForEach(x =>
            {
                try
                {
                    x.ConfigureServices(services);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, x);
                }
            });
        }

        /// <summary>
        /// 安装服务到启动类
        /// </summary>
        /// <param name="serviceProvider"></param>
        public void Configure(IServiceProvider serviceProvider)
        {
            //循环安装服务
            _startups.ForEach(x =>
            {
                try
                {
                    x.Configure(serviceProvider);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, x);
                }
            });
        }

    }
}
