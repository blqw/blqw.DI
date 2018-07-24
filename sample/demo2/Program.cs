using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demo2
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            var provider = services.BuildServiceProvider();
            var factory = provider.GetService<ILoggerFactory>();
            var x = new MyLoggerProvider();
            factory.AddProvider(x);
            factory.AddProvider(x);
            var logger = factory.CreateLogger("Ordering");
             logger = factory.CreateLogger("Ordering");

            using (logger.BeginScope("订单: {ID}", "20160520001"))
            {
                logger.LogWarning("商品库存不足(商品ID: {fdsafdsa}, 当前库存:{1}, 订购数量:{2})", "9787121237812", 20, 50);
                logger.LogError("商品ID录入错误(商品ID: {0})", "9787121235368");
            }
        }
    }
}
