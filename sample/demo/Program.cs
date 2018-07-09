using blqw;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

[assembly: AssemblyStartup(typeof(xxx.Startup))]

namespace demo
{

    class Program
    {
        static void Main(string[] args)
        {
            if (File.Exists("d:\\log.log"))
            {
                File.Delete("d:\\log.log");
            }
            //搜索整个应用程序域中"Startup 静态类"，忽略访问修饰符
            Startup.CreateServiceCollection()
                    .AddSingleton<ILogger>(new MyLogger("d:\\log.log"))
                    .AddConsoleLogger()
                    .ConsoleForwardingToLogger()
                    .ConfigureServices()
                    .ConfigureServices(AppDomain.CurrentDomain.FindStartupTypesByName())
                    .BuildServiceProvider()             //编译服务
                    .Configure();                       //安装服务

            Console.WriteLine("Console.WriteLine 输出到日志 0-9");
            for (var i = 0; i < 10; i++)
            {
                Console.WriteLine(i);
            }
        }
    }
}

namespace xxx
{

    static class Startup
    {

        private static string ToJsonString(object o) => o?.ToString() ?? "null";

        public static void ConfigureServices(/* 也可以没有参数 */IServiceCollection services)
        {
            //在这里注入组件
            services.AddNamedSingleton("tojson", ToJsonString);


            services.AddTransient(p => (Func<string, string>)(s => s + "_abc"));

            services.AddNamedSingleton("blqw", "12121212");
            services.AddNamedSingleton("blqw", 123456);
            services.AddNamedSingleton("blqw", "冰麟轻武");
        }

        public static void Configure(IServiceProvider provider)
        {
            var service4 = provider.GetNamedService<Func<object, string>>("tojson");
            Debug.Assert(service4(null) == "null");

            var service1 = provider.GetRequiredService<Func<string, string>>();
            Debug.Assert(service1("xx") == "xx_abc");

            var service2 = provider.GetRequiredNamedService<string>("blqw");
            Debug.Assert(service2 == "冰麟轻武");

            var service3 = provider.GetRequiredNamedService<int>("blqw");
            Debug.Assert(service3 == 123456);
        }
    }
}
