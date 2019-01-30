# blqw.DI.ExtensionMethods

扩展方法

## Demo


#### Autowired & CreateInstance
```csharp
static void Main(string[] args)
{
    var provider = new ServiceCollection()
                            .AddLogging()
                            .BuildServiceProvider();

    var a = provider.GetServiceOrCreateInstance<MyClass>();
    Console.WriteLine(a.Logger);

    var b = new MyClass();
    provider.Autowired(b);
    Console.WriteLine(b.Logger);

    provider.Autowired(typeof(MyClass));
}

class MyClass
{
    [Autowired]
    public ILogger<MyClass> Logger { get; }
    [Autowired]
    public ILogger<MyClass> Logger2 { get; private set; }
    [Autowired]
    private readonly ILogger<MyClass> _logger;

    [Autowired]
    public static ILogger<MyClass> Logger3 { get; }

    [Autowired]
    private readonly static ILogger<MyClass> _logger2;
}
```

## 

## 更新
###### [1.0.0] 2018.09.05 
* 初始版
* 新增 AutowiredAttribute
* 新增扩展方法 Autowired
* 新增扩展方法 CreateInstance等