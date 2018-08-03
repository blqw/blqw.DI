using blqw;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace demo
{

    public class Program
    {
        delegate string ToString(object obj);




        public static void AAA<T>()
        {
            if (_t == null)
            {
                _t = typeof(T);
            }
            else
            {
                Console.WriteLine(_t == typeof(T));
                _t = null;
            }
        }

        static Type _t;

        public static void BBB()
        {

        }
        static void Main(string[] args)
        {
            AAA<string>();
            AAA<string>();
            AAA<IEquatable<string>>();
            AAA<IEquatable<string>>();
            AAA<IEnumerable<string>>();
            AAA<IEnumerable<string>>();
            Console.WriteLine();






            var services = new ServiceCollection();
            //services.AddSingleton<Func<object, string>>(x => "string:" + x?.ToString());
            services.AddNamedSingleton<Func<object, string>>("hehe", x => "string:" + x?.ToString());
            var provider = services.BuildSupportDelegateServiceProvdier();
            //var toString = provider.GetService<ToString>();
            var toString = provider.GetNamedService<ToString>("hehe");
            toString = provider.GetNamedService<ToString>("hehe");
            toString = provider.GetNamedService<ToString>("hehe");
            toString = provider.GetNamedService<ToString>("hehe");
            Console.WriteLine(toString(new { id = 1, name = "blqw" }));


            if (File.Exists("d:\\log.log"))
            {
                File.Delete("d:\\log.log");
            }

            {
                var service5 = Startup.ServiceProvider.GetService<Func<int>>();
                Debug.Assert(service5() == 398398389);

                var service4 = Startup.ServiceProvider.GetNamedService<ToString>("tojson");
                Debug.Assert(service4(null) == "null");

                var service1 = Startup.ServiceProvider.GetRequiredService<Func<string, string>>();
                Debug.Assert(service1("xx") == "xx_abc");

                var service2 = Startup.ServiceProvider.GetRequiredNamedService<string>("blqw");
                Debug.Assert(service2 == "冰麟轻武");

                var service3 = Startup.ServiceProvider.GetRequiredNamedService<int>("blqw");
                Debug.Assert(service3 == 123456);
            }

            var p = blqw.Startup.CreateServiceCollection()
                    .AddLogging(x =>                    //安装日志框架
                    {
                        x.AddFilter(b => true);
                    })
                    //.ConfigureServices(AppDomain.CurrentDomain.FindStartupTypesByName()) //搜索整个应用程序域中名称为"Startup"的启动类，忽略访问修饰符
                    .ConfigureServices()                //添加 AssemblyStartupAttribute 特性标注的启动类
                    .BuildServiceProvider()             //编译服务
                                                        //.SupportDelegateConversion()        //支持委托转换
                    .AddConsoleLogger()                 //添加控制台日志
                    .TraceForwardingToLogger()          //使Trace输出内容(Trace.Wirte等)转发到日志
                    .Configure();                       //安装服务


            {
                var service4 = Startup.ServiceProvider.GetNamedService<Func<object, string>>("tojson");
                Debug.Assert(service4(null) == "null");

                var service1 = Startup.ServiceProvider.GetRequiredService<Func<string, string>>();
                Debug.Assert(service1("xx") == "xx_abc");

                var service2 = Startup.ServiceProvider.GetRequiredNamedService<string>("blqw");
                Debug.Assert(service2 == "冰麟轻武");

                var service3 = Startup.ServiceProvider.GetRequiredNamedService<int>("blqw");
                Debug.Assert(service3 == 123456);

            }
            var logger = p.GetLogger("Ordering");
            using (logger.BeginScope("订单: {ID}", "20160520001"))
            {
                logger.LogWarning("商品库存不足(商品ID: {fdsafdsa}, 当前库存:{1}, 订购数量:{2})", "9787121237812", 20, 50);
                logger.LogError("商品ID录入错误(商品ID: {0})", "9787121235368");
                logger.Log(LogLevel.Trace, 0, "1111", null, null);
            }

            Console.WriteLine("Console.WriteLine 输出到日志 0-9");
            for (var i = 0; i < 10; i++)
            {
                Console.WriteLine(i);
            }

            Trace.WriteLine("Trace.WriteLine 输出到日志 10-19");
            Trace.Flush();
            for (var i = 10; i < 20; i++)
            {
                Trace.WriteLine(i);
            }

            try
            {
                ((object)null).ToString();
            }
            catch (Exception ex)
            {
                Trace.TraceError("测试 异常:" + ex.ToString());
            }
            Console.WriteLine("按任意键退出");
            Console.ReadKey();
        }
    }
}