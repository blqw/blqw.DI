using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace blqw
{
    internal class StartupInvoker
    {
        public StartupInvoker(Type type, ILogger logger)
        {
            Type = type;
            _logger = logger;
            var flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            _configureServices = type.GetMethod("ConfigureServices", flags);
            _configure = type.GetMethod("Configure", flags);
            if (_configureServices?.IsStatic == false || _configure?.IsStatic == false)
            {
                try
                {
                    Instance = Activator.CreateInstance(type);
                    _logger?.LogDebug(new EventId(1, "CreateInstance"), type.FullName + " 创建成功");
                }
                catch (Exception ex)
                {
                    _logger?.LogError(new EventId(1, "CreateInstance"), ex, type.FullName + " 创建失败");
                    Type = null;
                }
            }
        }

        public Type Type { get; }

        public object Instance { get; }

        private readonly MethodInfo _configureServices;

        private readonly MethodInfo _configure;
        private readonly ILogger _logger;

        public void ConfigureServices(IServiceCollection services)
        {
            if (Type != null && _configureServices != null)
            {
                try
                {
                    var p = _configureServices.GetParameters();
                    var obj = _configureServices.IsStatic ? null : Instance;
                    if (p.Length == 0)
                    {
                        _configureServices.Invoke(obj, null);
                    }
                    else if (p.Length == 1 && typeof(IServiceCollection).IsAssignableFrom(p[0].ParameterType))
                    {
                        _configureServices.Invoke(obj, new object[] { services });
                    }
                    _logger?.LogDebug(new EventId(2, "ConfigureServices"), Type.FullName + " 注册服务成功");
                }
                catch (Exception ex)
                {
                    _logger?.LogError(new EventId(2, "ConfigureServices"), ex, Type.FullName + " 注册服务失败");
                }
            }
        }

        public void Configure(IServiceProvider serviceProvider)
        {
            if (Type != null && _configure != null)
            {
                try
                {
                    var obj = _configure.IsStatic ? null : Instance;
                    var args = _configure.GetParameters()
                                    .Select(x => serviceProvider.GetService(x.ParameterType))
                                    .ToArray();
                    _configure.Invoke(obj, args);
                    _logger?.LogDebug(new EventId(2, "Configure"), Type.FullName + " 安装完成");
                }
                catch (Exception ex)
                {
                    _logger?.LogError(new EventId(2, "Configure"), ex, Type.FullName + " 安装服务失败");
                }
            }
        }
    }
}
