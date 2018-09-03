using blqw;
using blqw.DI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Newtonsoft.Json;
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

        static void Main(string[] args)
        {
            var provider = new ServiceCollection()
                                    .AddSingleton(typeof(IList<>), typeof(List<>))
                                    .BuildServiceProvider();
            var list1 = provider.GetService(typeof(IList<string>));
            var list2 = provider.GetService(typeof(IList<int>));
            Console.WriteLine((list1, list2));

        }
    }

}