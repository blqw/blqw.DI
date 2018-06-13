using blqw;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: AssemblyStartup(typeof(xxx.Startup))]

namespace demo
{

    class Program
    {
        static void Main(string[] args)
        {
            //搜索整个应用程序域中"Startup 静态类"，忽略访问修饰符
            //调用静态类中的 ConfigureServices 方法
            var services = Startup.ConfigureServicesWithAttribute();
            //调用静态类中的 Configure 方法
            Startup.Configure(services.BuildServiceProvider());
        }
    }
}

namespace xxx
{

    static class Startup
    {
        public static void ConfigureServices(/* 也可以没有参数 */IServiceCollection services)
        {
            //在这里注入组件
            services.AddTransient(p => (Func<string, string>)(s => s + "_abc"));
            services.AddNamedSingleton("blqw", "12121212");
            services.AddNamedSingleton("blqw", 123456);
            services.AddNamedSingleton("blqw", "冰麟轻武");
        }

        public static void Configure(IServiceProvider provider)
        {
            var service1 = provider.GetRequiredService<Func<string, string>>();
            Console.WriteLine(service1("xx") == "xx_abc");

            var service2 = provider.GetRequiredNamedService<string>("blqw");
            Console.WriteLine(service2 == "冰麟轻武");


            var service3 = provider.GetRequiredNamedService<int>("blqw");
            Console.WriteLine(service3 == 123456);
        }
    }
}
