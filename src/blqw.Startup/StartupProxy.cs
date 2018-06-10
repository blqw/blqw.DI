using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace blqw
{
    /// <summary>
    /// 启动器代理程序
    /// </summary>
    internal class StartupProxy
    {
        public StartupProxy(Type setupType, ILogger logger)
        {
            SetupType = setupType ?? throw new ArgumentNullException(nameof(setupType));
            _logger = logger;
            const BindingFlags FLAGS = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            _configureServices = setupType.GetMethod("ConfigureServices", FLAGS);
            _configure = setupType.GetMethod("Configure", FLAGS);
            if (_configureServices?.IsStatic == false || _configure?.IsStatic == false)
            {
                try
                {
                    SetupInstance = Activator.CreateInstance(setupType);
                    _logger?.LogDebug(new EventId(1, "CreateInstance"), setupType.FullName + " 创建成功");
                }
                catch (Exception ex)
                {
                    _logger?.LogError(new EventId(1, "CreateInstance"), ex, setupType.FullName + " 创建失败");
                    SetupType = null;
                }
            }
        }

        /// <summary>
        /// 启动器类型
        /// </summary>
        public Type SetupType { get; }

        /// <summary>
        /// 启动器实例
        /// </summary>
        public object SetupInstance { get; }

        private readonly MethodInfo _configureServices;
        private readonly MethodInfo _configure;
        private readonly ILogger _logger;

        public void ConfigureServices(IServiceCollection services)
        {
            if (_configureServices == null)
            {
                return;
            }

            try
            {
                var p = _configureServices.GetParameters();
                var obj = _configureServices.IsStatic ? null : SetupInstance;
                if (p.Length == 0)
                {
                    _configureServices.Invoke(obj, null);
                }
                else if (p.Length == 1 && typeof(IServiceCollection).IsAssignableFrom(p[0].ParameterType))
                {
                    _configureServices.Invoke(obj, new object[] { services });
                }
                _logger?.LogDebug(new EventId(2, "ConfigureServices"), SetupType.FullName + " 注册服务成功");
            }
            catch (Exception ex)
            {
                _logger?.LogError(new EventId(2, "ConfigureServices"), ex, SetupType.FullName + " 注册服务失败");
            }
        }

        public void Configure(IServiceProvider serviceProvider)
        {
            if (SetupType != null && _configure != null)
            {
                try
                {
                    var obj = _configure.IsStatic ? null : SetupInstance;
                    var args = _configure.GetParameters()
                                    .Select(x => serviceProvider.GetService(x.ParameterType))
                                    .ToArray();
                    _configure.Invoke(obj, args);
                    _logger?.LogDebug(new EventId(2, "Configure"), SetupType.FullName + " 安装完成");
                }
                catch (Exception ex)
                {
                    _logger?.LogError(new EventId(2, "Configure"), ex, SetupType.FullName + " 安装服务失败");
                }
            }
        }
    }
}
