using blqw;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

[assembly: AssemblyStartup(typeof(Startup))]
static class Startup
{
    #region hide
    /// <summary>
    /// 服务提供程序
    /// </summary>
    private static IServiceProvider _serviceProvider;
    /// <summary>
    /// 服务提供程序更新事件
    /// </summary>
    private static event EventHandler<IServiceProvider> _changed;

    /// <summary>
    /// 主动生成一个 <seealso cref="IServiceProvider"/>
    /// </summary>
    /// <returns></returns>
    private static IServiceProvider Initiative()
    {
        if (_serviceProvider == null)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildSupportDelegateServiceProvdier();
            if (Interlocked.CompareExchange(ref _serviceProvider, serviceProvider, null) == null)
            {
                OnChanged(serviceProvider);
            }
        }
        return _serviceProvider;
    }


    /// <summary>
    /// 更新组件事件
    /// </summary>
    public static event EventHandler<IServiceProvider> ServiceProviderChanged
    {
        add
        {
            _changed -= value;
            _changed += value;
        }
        remove
        {
            _changed -= value;
        }
    }

    /// <summary>
    /// 触发 <seealso cref="ServiceProviderChanged"/> 事件
    /// </summary>
    /// <param name="convertors"></param>
    private static void OnChanged(IServiceProvider serviceProvider) =>
        _changed?.Invoke(null, serviceProvider);


    /// <summary>
    /// 服务提供程序, 优先获取注入的服务转换器, 如果没有注入则获取内部实现的服务提供程序
    /// </summary>
    public static IServiceProvider ServiceProvider => _serviceProvider ?? Initiative();

    #endregion

    /// <summary>
    /// 配置服务
    /// </summary>
    /// <param name="services"></param>
    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddNamedSingleton("tojson", ToJsonString);
        services.AddSingleton(typeof(Startup).GetMethod("GetInt", (System.Reflection.BindingFlags)(-1)));


        services.AddTransient(p => (Func<string, string>)(s => s + "_abc"));

        services.AddNamedSingleton("blqw", "12121212");
        services.AddNamedSingleton("blqw", 123456);
        services.AddNamedSingleton("blqw", "冰麟轻武");
    }

    /// <summary>
    /// 安装服务
    /// </summary>
    private static void Configure(IServiceProvider serviceProvider)
    {
        var prev = Interlocked.Exchange(ref _serviceProvider, serviceProvider);
        if (prev != serviceProvider)
        {
            OnChanged(serviceProvider);
        }
        //TODO: 这里获取服务
    }

    private static string ToJsonString(object o) => o?.ToString() ?? "null";
    private static int GetInt() => 398398389;
}