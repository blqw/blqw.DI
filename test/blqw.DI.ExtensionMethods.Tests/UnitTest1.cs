using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using Xunit;

namespace blqw.DI.ExtensionMethods.Tests
{
    public class UnitTest1
    {
        class MyEntity
        {

            [Autowrite] IServiceProvider _serviceProvider;

            [Autowrite] readonly IServiceProvider _serviceProviderReadOnly;

            [Autowrite] static IServiceProvider _staticServiceProvider;

            [Autowrite] readonly static IServiceProvider _staticServiceProviderReadOnly;

            [Autowrite] public IServiceProvider PublicServiceProvider;

            [Autowrite] public static IServiceProvider PublicStaticServiceProvider;

            [Autowrite] public IServiceProvider ServiceProvider { get; set; }

            [Autowrite] public IServiceProvider ServiceProviderReadOnly { get; }

            [Autowrite] public IServiceProvider ServiceProviderPrivateSet { get; private set; }

            [Autowrite] public static IServiceProvider StaticServiceProvider { get; set; }

            [Autowrite] public static IServiceProvider StaticServiceProviderPrivateSet { get; private set; }

            [Autowrite] public static IServiceProvider StaticServiceProviderReadOnly { get; }


            public static IServiceProvider NoAutoWrite1 { get; set; }
            public IServiceProvider NoAutoWrite2 { get; set; }
            public static IServiceProvider NoAutoWrite3;
            public IServiceProvider NoAutoWrite4;

        }
        [Fact]
        public void Autowrite()
        {
            var provider = new ServiceCollection().BuildServiceProvider();
            var p = provider.GetService<IServiceProvider>();

            var test = provider.CreateInstance<MyEntity>();
            foreach (var field in test.GetType().GetFields((BindingFlags)(-1)))
            {
                if (field.Name.StartsWith("<") || field.IsStatic)
                {
                    continue;
                }
                if (field.IsDefined(typeof(AutowriteAttribute)))
                {
                    Assert.Equal(field.GetValue(test), p);
                }
                else
                {
                    Assert.Null(field.GetValue(test));
                }
            }
            foreach (var prop in test.GetType().GetProperties((BindingFlags)(-1)))
            {
                if (prop.Name.StartsWith("<") || prop.GetMethod.IsStatic)
                {
                    continue;
                }
                if (prop.IsDefined(typeof(AutowriteAttribute)))
                {
                    Assert.Equal(prop.GetValue(test), p);
                }
                else
                {
                    Assert.Null(prop.GetValue(test));
                }
            }

            using (var scope = provider.CreateScope())
            {
                scope.ServiceProvider.Autowrite(test);
                p = scope.ServiceProvider.GetService<IServiceProvider>();
                foreach (var field in test.GetType().GetFields((BindingFlags)(-1)))
                {
                    if (field.Name.StartsWith("<") || field.IsStatic)
                    {
                        continue;
                    }
                    if (field.IsDefined(typeof(AutowriteAttribute)))
                    {
                        Assert.Equal(field.GetValue(test), p);
                    }
                    else
                    {
                        Assert.Null(field.GetValue(test));
                    }
                }
                foreach (var prop in test.GetType().GetProperties((BindingFlags)(-1)))
                {
                    if (prop.Name.StartsWith("<") || prop.GetMethod.IsStatic)
                    {
                        continue;
                    }
                    if (prop.IsDefined(typeof(AutowriteAttribute)))
                    {
                        Assert.Equal(prop.GetValue(test), p);
                    }
                    else
                    {
                        Assert.Null(prop.GetValue(test));
                    }
                }
                provider.Autowrite(test);
            }
        }


        [Fact]
        public void AutowriteStatic()
        {
            var provider = new ServiceCollection().BuildServiceProvider();
            var p = provider.GetService<IServiceProvider>();
            var type = typeof(MyEntity);
            provider.Autowrite(type);
            foreach (var field in type.GetFields((BindingFlags)(-1)))
            {
                if (field.Name.StartsWith("<") || !field.IsStatic)
                {
                    continue;
                }
                if (field.IsDefined(typeof(AutowriteAttribute)))
                {
                    Assert.Equal(field.GetValue(null), p);
                }
                else
                {
                    Assert.Null(field.GetValue(null));
                }
            }
            foreach (var prop in type.GetProperties((BindingFlags)(-1)))
            {
                if (prop.Name.StartsWith("<") || !prop.GetMethod.IsStatic)
                {
                    continue;
                }
                if (prop.IsDefined(typeof(AutowriteAttribute)))
                {
                    Assert.Equal(prop.GetValue(null), p);
                }
                else
                {
                    Assert.Null(prop.GetValue(null));
                }
            }

            using (var scope = provider.CreateScope())
            {
                scope.ServiceProvider.Autowrite(type);
                p = scope.ServiceProvider.GetService<IServiceProvider>();
                foreach (var field in type.GetFields((BindingFlags)(-1)))
                {
                    if (field.Name.StartsWith("<") || !field.IsStatic)
                    {
                        continue;
                    }
                    if (field.IsDefined(typeof(AutowriteAttribute)))
                    {
                        Assert.Equal(field.GetValue(null), p);
                    }
                    else
                    {
                        Assert.Null(field.GetValue(null));
                    }
                }
                foreach (var prop in type.GetProperties((BindingFlags)(-1)))
                {
                    if (prop.Name.StartsWith("<") || !prop.GetMethod.IsStatic)
                    {
                        continue;
                    }
                    if (prop.IsDefined(typeof(AutowriteAttribute)))
                    {
                        Assert.Equal(prop.GetValue(null), p);
                    }
                    else
                    {
                        Assert.Null(prop.GetValue(null));
                    }
                }
                provider.Autowrite(type);
            }
        }
    }
}
