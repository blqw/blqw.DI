using blqw.DI;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Linq;
namespace demo
{
    class DelegateService
    {
        public static void Demo()
        {
            var provider = new ServiceCollection()
                   .AddSingleton<Func<object, string>>(o => JsonConvert.SerializeObject(o))
                   .AddSingleton<ToJsonString>(o => JsonConvert.SerializeObject(o))
                   .AddSingleton<Func<object, string>>(p => (Func<object, string>)JsonConvert.SerializeObject)
                   .AddSingleton(typeof(JsonConvert).GetMethod("SerializeObject", new[] { typeof(object) }))
                   .AddSingleton(p => typeof(JsonConvert).GetMethod("SerializeObject", new[] { typeof(object) }))
                   .AddNamedSingleton<ToJsonString>("ToJsonString", o => JsonConvert.SerializeObject(o))
                   .AddNamedSingleton("ToJsonString", typeof(JsonConvert).GetMethod("SerializeObject", new[] { typeof(object) }))
                   .AddNamedSingleton<Func<object, string>>("ToJsonString", o => JsonConvert.SerializeObject(o))
                   .BuildSupportDelegateServiceProvdier();

            Business.Operation(provider);
        }

        delegate string ToJsonString(object obj);
        static class Business
        {
            public static void Operation(IServiceProvider provider)
            {


                var x = new
                {
                    id = 1,
                    name = "blqw"
                };
                var toJsonStriongs = provider.GetServices<ToJsonString>();
                Assert(toJsonStriongs.Count() == 5);
                foreach (var s in toJsonStriongs)
                {
                    Console.WriteLine(s(x));
                    Console.WriteLine("------");
                }

                var toJsonStriongs2 = provider.GetNamedServices<ToJsonString>("ToJsonString");
                Assert(toJsonStriongs2.Count() == 3);
                foreach (var s in toJsonStriongs2)
                {
                    Console.WriteLine(s(x));
                    Console.WriteLine("------");
                }

                var toJsonStriongs3 = provider.GetNamedService("ToJsonString");
                Assert(toJsonStriongs3 != null);
                Console.WriteLine(toJsonStriongs3);
                Console.WriteLine("------");


                var toJsonStriongs4 = provider.GetNamedService<ToJsonString>("ToJsonString");
                Assert(toJsonStriongs4 != null);
                Console.WriteLine(toJsonStriongs4);
                Console.WriteLine("------");
            }

            private static void Assert(bool condition)
            {
                if (!condition)
                {
                    throw new Exception();
                }
            }
        }
    }
}
