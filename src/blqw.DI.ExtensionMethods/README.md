# blqw.DI.ExtensionMethods

扩展方法

## Demo


#### Autowrite & CreateInstance
```csharp
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
```

## 

## 更新
###### [1.0.0] 2018.09.05 
* 初始版
* 新增 AutowriteAttribute
* 新增扩展方法 Autowrite
* 新增扩展方法 CreateInstance等