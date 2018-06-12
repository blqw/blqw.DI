using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using blqw;
using Microsoft.Extensions.DependencyInjection;

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
        }

        public static void Configure(/* 也可以没有参数 */Func<string, string> get)
        {
            //在这里使用已注入的组件
            Console.WriteLine(get("xxx.Configure"));
        }
    }
}
