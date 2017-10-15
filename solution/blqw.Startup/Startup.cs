using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace blqw
{
    /// <summary>
    /// 
    /// </summary>
    public static class Startup
    {
        private static int _flag = 0;

        /// <summary>
        /// 执行中
        /// </summary>
        public static bool IsRunning => _flag == 1;
        /// <summary>
        /// 执行完成
        /// </summary>
        public static bool IsCompleted => _flag == 2;


        private class Invoker
        {
            public Invoker(Type type)
            {
                Type = type;
                var flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
                _configureServices = type.GetMethod("ConfigureServices", flags);
                _configure = type.GetMethod("Configure", flags);
                if (_configureServices.IsStatic == false || _configure.IsStatic == false)
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
                        var obj = _configureServices.IsStatic ? null : Instance;
                        var args = _configureServices.GetParameters()
                                        .Select(x => serviceProvider.GetService(x.ParameterType))
                                        .ToArray();
                        _configureServices.Invoke(obj, args);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }


        private static List<Invoker> _invokers;

        private static List<Invoker> Invokers
            => _invokers ?? (_invokers = AppDomain.CurrentDomain.GetAssemblies()
                                        .SelectMany(x => x.Modules)
                                        .SelectMany(x => x.GetTypes())
                                        .Where(x => x.Name == "Startup")
                                        .Select(x => new Invoker(x))
                                        .Where(x => x.Type != null)
                                        .ToList());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        public static void ConfigureServices(IServiceCollection services)
        {
            var value = System.Threading.Interlocked.CompareExchange(ref _flag, 1, 0);
            if (value != 0)
            {
                return;
            }

            Invokers.ForEach(x => x.ConfigureServices(services));
            System.Threading.Interlocked.Exchange(ref _flag, 2);
        }


        public static void Configure(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                Invokers.ForEach(x => x.Configure(scope.ServiceProvider));
            }
        }
    }
}
