using System.Reflection;
using System.Security;
using blqw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Startup.ConfigureServices(null);
            Startup.Configure(null);
        }
    }
}

namespace xxx
{
    public delegate string GetString(object s);
    static class Startup
    {
        public static string GetString(string s, string s1) => s + "_string";
        public static string GetString(object s) => s + "_object";

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient(p => (Func<string, string, string>)GetString);
            services.AddTransient(p => (Func<string, string>)GetString);
        }

        public static void Configure(GetString get)
        {
            Console.WriteLine(get("xxx.Configure"));
        }
    }
}

namespace yyy
{
    class Startup
    {
        public void ConfigureServices()
        {
            Console.WriteLine("yyy.ConfigureServices");
        }
    }
}