# blqw.DI.Startup



## Demo
```cs
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
}
```
```cs
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
```

https://www.jianshu.com/p/ae8991280fb5

## 更新

###### [1.0.0] 
* 初始版