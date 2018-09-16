using blqw;
using blqw.DI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;


[assembly:AssemblyStartup(typeof(demo.Startup))]
namespace demo
{
    class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine("ConfigureServices");
        }

        public void Configure(ILogger<Startup> logger)
        {
            Console.WriteLine("Configure");
            Console.WriteLine("logger: " + logger);
        }
    }

    public class Program
    {

        static void Main(string[] args)
        {
            var provider = new ServiceCollection()
                                    .AddLogging()
                                    .ConfigureServices()     //调用 启动类的 ConfigureServices
                                    .BuildServiceProvider()
                                    .Configure();            //调用 启动类的 Configure

        }

    }
}