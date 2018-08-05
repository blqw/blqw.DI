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
                    .AddSingleton<Func<object, string>>(o => JsonConvert.SerializeObject(o)) //注入
                    .BuildSupportDelegateServiceProvdier();

            Business.Operation(provider);
        }
    }


    static class Business
    {
        delegate string ToJsonString(object obj);
        public static void Operation(IServiceProvider provider)
        {
            var x = new
            {
                id = 1,
                name = "blqw"
            };
            var toJsonStriong = provider.GetService<ToJsonString>();
            Console.WriteLine(toJsonStriong(x));
        }
    }
}