using blqw;
using Microsoft.Extensions.DependencyInjection;
using System;

/// <summary>
/// 添加和使用静态 Startup 类的方法
/// </summary>
public static class BlqwStartupExtensions
{
    /// <summary>
    /// 添加静态 Startup 类中的服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="filter">过滤器</param>
    /// <remarks>调用静态类 Startup 中的 ConfigureServices 方法</remarks>
    public static IServiceCollection AddStaticStartup(this IServiceCollection services, Func<Type, bool> filter = null)
    {
        if (services == null)
        {
            return null;
        }
        Startup.ConfigureServices(services, filter);
        return services;
    }

    /// <summary>
    /// 启用静态 Startup 类中的服务
    /// </summary>
    /// <param name="serviceProvider">服务提供程序</param>
    /// <param name="args">额外参数</param>
    public static void UseStaticStartup(this IServiceProvider serviceProvider, params object[] args) 
        => Startup.Configure(serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider)), args);
}

