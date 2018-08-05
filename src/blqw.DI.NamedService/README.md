# blqw.NamedService

在微软DI框架的基础上扩展，以支持命名服务

## Demo
```cs
class Program
{
    static void Main(string[] args)
    {
        var provider = new ServiceCollection()
                     .AddNamedSingleton<Func<object, string>>("ToJsonString", o => JsonConvert.SerializeObject(o)) //注入
                     .AddNamedSingleton<Func<object, string>>("ToXmlString", o => o.ToXml().ToString())
                     .BuildServiceProvider();

        CallContext.SetData("ServiceProvider", provider);

        Business.Operation();
    }
}
```

```cs
static class Business
{
    public static void Operation()
    {
        var x = new
        {
            id = 1,
            name = "blqw"
        };
        var provider = ((IServiceProvider)CallContext.GetData("ServiceProvider"));
        var toJsonStriong = provider.GetNamedService<Func<object, string>>("ToJsonString");
        Console.WriteLine(toJsonStriong(x));
        var toXmlString = provider.GetNamedService<Func<object, string>>("ToXmlString");
        Console.WriteLine(toXmlString(x));
    }
}
```

https://www.jianshu.com/p/ae8991280fb5