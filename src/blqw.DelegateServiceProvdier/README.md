# blqw.DelegateServiceProvdier

支持不同类型之间委托/方法类型的服务的转换

## Demo
```cs
public class Program
{
    static void Main(string[] args)
    {
        var provider = new ServiceCollection()
                            .AddSingleton<Func<object, string>>(o => JsonConvert.SerializeObject(o)) //注入 Func<object, string>
                            .BuildSupportDelegateServiceProvdier();  // 支持委托转换
        Business.Operation(provider);
    }
}
```
```cs
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
        // 获取 delegate string ToJsonString(object obj);
        var toJsonStriong = provider.GetService<ToJsonString>();  
        Console.WriteLine(toJsonStriong(x));
    }
}
```

## 与命名服务同时使用
https://github.com/blqw/blqw.DI/tree/feature/2.x/src/blqw.NamedService
```cs
public class Program
{
    static void Main(string[] args)
    {
        var provider = new ServiceCollection()
                            .AddNamedSingleton<Func<object, string>>("ToJsonString", o => JsonConvert.SerializeObject(o)) //注入 Func<object, string>
                            .BuildSupportDelegateServiceProvdier();  // 支持委托转换
        Business.Operation(provider);
    }
}
```
```cs
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
        // 获取 delegate string ToJsonString(object obj);
        var toJsonStriong = provider.GetNamedService<ToJsonString>("ToJsonString");  
        Console.WriteLine(toJsonStriong(x));
    }
}
```