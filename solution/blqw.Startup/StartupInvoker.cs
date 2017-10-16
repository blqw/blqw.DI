using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace blqw
{
    internal class StartupInvoker
    {
        public StartupInvoker(Type type)
        {
            Type = type;
            var flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            _configureServices = type.GetMethod("ConfigureServices", flags);
            _configure = type.GetMethod("Configure", flags);
            if (_configureServices?.IsStatic == false || _configure?.IsStatic == false)
            {
                try
                {
                    Instance = Activator.CreateInstance(type);
                }
                catch (Exception)
                {
                    Type = null;
                }
            }
        }

        public Type Type { get; }

        public object Instance { get; }

        private readonly MethodInfo _configureServices;

        private readonly MethodInfo _configure;

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
                }
                catch (Exception)
                {

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
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
