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

namespace demo
{

    public class Program
    {

        static void Main(string[] args)
        {
            var provider = new ServiceCollection()
                                    .AddLogging()
                                    .BuildServiceProvider();

            var a = provider.GetServiceOrCreateInstance<MyClass>();
            Console.WriteLine(a.Logger);

            var b = new MyClass();
            provider.Autowrite(b);
            Console.WriteLine(b.Logger);

            provider.Autowrite(typeof(MyClass));
        }

        class MyClass
        {
            [Autowrite]
            public ILogger<MyClass> Logger { get; }
            [Autowrite]
            public ILogger<MyClass> Logger2 { get; private set; }
            [Autowrite]
            private readonly ILogger<MyClass> _logger;

            [Autowrite]
            public static ILogger<MyClass> Logger3 { get; }

            [Autowrite]
            private readonly static ILogger<MyClass> _logger2;
        }
    }


}