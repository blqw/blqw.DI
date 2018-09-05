# blqw.DI.StartupAttribute

启动类（Startup.cs）建议写法

```cs
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
            var serviceProvider = services.BuildServiceProvider();
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
        //TODO: 这里注入服务
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
}
```

## 更新

###### [1.0.1] 2018.08.05
* 调整命名空间

###### [1.0.0] 2018.08.01
* 初始版