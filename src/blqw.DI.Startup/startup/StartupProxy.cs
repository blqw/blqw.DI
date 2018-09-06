using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;

namespace blqw.DI
{
    /// <summary>
    /// 启动器代理程序
    /// </summary>
    internal class StartupProxy : IStartup, ILoggable
    {
        /// <summary>
        /// 初始化启动器代理
        /// </summary>
        /// <param name="startupType">启动器类</param>
        public StartupProxy(Type startupType)
        {
            StartupType = startupType ?? throw new ArgumentNullException(nameof(startupType));
            const BindingFlags FLAGS = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            var methods = startupType.GetMethods(FLAGS).Where(x => !x.IsGenericMethod || !x.IsGenericMethodDefinition);
            _configureServices = methods.FirstOrDefault(x => x.Name == "ConfigureServices");
            _configure = methods.FirstOrDefault(x => x.Name == "Configure");
            if (_configureServices?.IsStatic == false || _configure?.IsStatic == false) //2个方法中有任何一个方法是实例方法
            {
                try
                {
                    SetupInstance = Activator.CreateInstance(startupType);
                }
                catch (Exception ex)
                {
                    if (_configureServices.IsStatic == false)
                    {
                        _configureServices = null;
                    }
                    if (_configure.IsStatic == false)
                    {
                        _configure = null;
                    }
                    StartupType = null;
                    Exception = ex;
                }
            }
        }

        /// <summary>
        /// 无效代理的错误原因
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// 启动器类型
        /// </summary>
        public Type StartupType { get; }

        /// <summary>
        /// 启动器实例
        /// </summary>
        public object SetupInstance { get; }

        private readonly MethodInfo _configureServices;
        private readonly MethodInfo _configure;
        public ILogger Logger { get; set; }

        /// <summary>
        /// 调用被代理对象的配置服务方法
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            if (_configureServices == null || _configureServices == null)
            {
                return;
            }

            try
            {
                var p = _configureServices.GetParameters();
                var obj = _configureServices.IsStatic ? null : SetupInstance;
                Logger.Log("配置服务:" + StartupType.FullName);
                if (p.Length == 0)
                {
                    _configureServices.Invoke(obj, null);
                }
                else if (p.Length == 1 && typeof(IServiceCollection).IsAssignableFrom(p[0].ParameterType))
                {
                    _configureServices.Invoke(obj, new object[] { services });
                }
                else
                {
                    Logger.Error(null, StartupType.FullName + " 配置服务失败:ConfigureServices 方法签名错误");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, StartupType.FullName + " 配置服务失败");
            }
        }

        /// <summary>
        /// 调用被代理对象的安装服务方法
        /// </summary>
        /// <param name="serviceProvider"></param>
        public void Configure(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null || _configure == null)
            {
                return;
            }
            try
            {
                Logger.Log("安装服务:" + StartupType.FullName);
                var obj = _configure.IsStatic ? null : SetupInstance;
                var args = _configure.GetParameters().Select(x => serviceProvider.GetParameterValue(x)).ToArray();
                _configure.Invoke(obj, args);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, StartupType.FullName + " 安装服务失败");
            }
        }
    }
}
