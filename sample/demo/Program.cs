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

            Startup.CreateServiceCollection()
                    .AddSingleton<ILogger>(new MyLogger("d:\\log.log")) //添加自定义日志组件
                    .AddConsoleLogger()             //添加控制台日志
                    .ConsoleForwardingToLogger()    //使控制台输出内容(Console.Wirte)转发到日志
                    .TraceForwardingToLogger()      //使Trace输出内容(Trace.Wirte等)转发到日志
                    .ConfigureServices()            //添加 AssemblyStartupAttribute 特性标注的启动类
                    //.ConfigureServices(AppDomain.CurrentDomain.FindStartupTypesByName()) //搜索整个应用程序域中名称为"Startup"的启动类，忽略访问修饰符
                    .BuildServiceProvider()             //编译服务
                    .Configure();                       //安装服务

            Console.WriteLine("Console.WriteLine 输出到日志 0-9");
            for (var i = 0; i < 10; i++)
            {
                Console.WriteLine(i);
            }

            Trace.WriteLine("Trace.WriteLine 输出到日志 10-19");
            for (var i = 10; i < 20; i++)
            {
                Trace.WriteLine(i);
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
