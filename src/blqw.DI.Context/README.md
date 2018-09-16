# blqw.DI.Context


## Demo
```cs
var provider = ServiceContextFactory.Create(services.BuildServiceProvider());

var b = ReferenceEquals(provider, ServiceContext.Provider);
// b = true

using(var scope = provider.CreateScope())
{
    b = ReferenceEquals(scope.Provider, ServiceContext.Provider);
    // b = true
}

var b = ReferenceEquals(provider, ServiceContext.Provider);
// b = true

```

[https://www.jianshu.com/p/e8fdc39a2d9f](https://www.jianshu.com/p/e8fdc39a2d9f)

## 更新

###### [1.0.0] 2018.09.16
* 初始版