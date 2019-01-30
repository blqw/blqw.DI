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

            [Autowired] IServiceProvider _serviceProvider;

            [Autowired] readonly IServiceProvider _serviceProviderReadOnly;

            [Autowired] static IServiceProvider _staticServiceProvider;

            [Autowired] readonly static IServiceProvider _staticServiceProviderReadOnly;

            [Autowired] public IServiceProvider PublicServiceProvider;

            [Autowired] public static IServiceProvider PublicStaticServiceProvider;

            [Autowired] public IServiceProvider ServiceProvider { get; set; }

            [Autowired] public IServiceProvider ServiceProviderReadOnly { get; }

            [Autowired] public IServiceProvider ServiceProviderPrivateSet { get; private set; }

            [Autowired] public static IServiceProvider StaticServiceProvider { get; set; }

            [Autowired] public static IServiceProvider StaticServiceProviderPrivateSet { get; private set; }

            [Autowired] public static IServiceProvider StaticServiceProviderReadOnly { get; }


            public static IServiceProvider NoAutoWrite1 { get; set; }
            public IServiceProvider NoAutoWrite2 { get; set; }
            public static IServiceProvider NoAutoWrite3;
            public IServiceProvider NoAutoWrite4;

        }
        [Fact]
        public void Autowired()
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
                if (field.IsDefined(typeof(AutowiredAttribute)))
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
                if (prop.IsDefined(typeof(AutowiredAttribute)))
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
                scope.ServiceProvider.Autowired(test);
                p = scope.ServiceProvider.GetService<IServiceProvider>();
                foreach (var field in test.GetType().GetFields((BindingFlags)(-1)))
                {
                    if (field.Name.StartsWith("<") || field.IsStatic)
                    {
                        continue;
                    }
                    if (field.IsDefined(typeof(AutowiredAttribute)))
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
                    if (prop.IsDefined(typeof(AutowiredAttribute)))
                    {
                        Assert.Equal(prop.GetValue(test), p);
                    }
                    else
                    {
                        Assert.Null(prop.GetValue(test));
                    }
                }
                provider.Autowired(test);
            }
        }


        [Fact]
        public void AutowiredStatic()
        {
            var provider = new ServiceCollection().BuildServiceProvider();
            var p = provider.GetService<IServiceProvider>();
            var type = typeof(MyEntity);
            provider.Autowired(type);
            foreach (var field in type.GetFields((BindingFlags)(-1)))
            {
                if (field.Name.StartsWith("<") || !field.IsStatic)
                {
                    continue;
                }
                if (field.IsDefined(typeof(AutowiredAttribute)))
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
                if (prop.IsDefined(typeof(AutowiredAttribute)))
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
                scope.ServiceProvider.Autowired(type);
                p = scope.ServiceProvider.GetService<IServiceProvider>();
                foreach (var field in type.GetFields((BindingFlags)(-1)))
                {
                    if (field.Name.StartsWith("<") || !field.IsStatic)
                    {
                        continue;
                    }
                    if (field.IsDefined(typeof(AutowiredAttribute)))
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
                    if (prop.IsDefined(typeof(AutowiredAttribute)))
                    {
                        Assert.Equal(prop.GetValue(null), p);
                    }
                    else
                    {
                        Assert.Null(prop.GetValue(null));
                    }
                }
                provider.Autowired(type);
            }
        }
    }
}
