# blqw.Startup

可以在项目启动时调用项目中所有的Startup静态类

## Demo
```cs
class Program
{
    static void Main (string[] args)
    {
        //搜索整个应用程序域中"Startup 静态类"，忽略访问修饰符
        //调用静态类中的 ConfigureServices 方法
        Startup.ConfigureServices (null);
        //调用静态类中的 Configure 方法
        Startup.Configure (null);
    }
}


static class Startup
{
    //在这里注入组件
    public static void ConfigureServices (/* 也可以没有参数 */IServiceCollection services)
    {
        services.AddTransient (p => (Func<string, string>) (s => s + "_abc"));
    }

    //在这里使用已注入的组件
    public static void Configure (/* 也可以没有参数 */Func<string, string> get)
    {
        if(get != null){
            Console.WriteLine (get ("xxx.Configure"));
        }        
    }
}
```

> 下一个版本支持 `static void ConfigureServices(IContainer container)`