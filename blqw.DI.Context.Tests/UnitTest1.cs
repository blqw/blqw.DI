using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Xunit;

namespace blqw.DI.Context.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void AddContext()
        {
            {
                var provider = new ServiceCollection()
                                    .BuildServiceProvider()
                                    .RebuildFromFactory();

                Assert.Null(ServiceContext.Provider);
            }

            {
                var provider = new ServiceCollection()
                                    .AddContext()
                                    .BuildServiceProvider();

                Assert.Null(ServiceContext.Provider);
            }

            {
                var provider = new ServiceCollection()
                                    .AddContext()
                                    .BuildServiceProvider()
                                    .RebuildFromFactory();
                Assert.NotNull(ServiceContext.Provider);
                Assert.Equal(ServiceContext.Provider, provider);
            }

            {

                var provider = ServiceContextFactory.Create(new ServiceCollection().BuildServiceProvider());

                Assert.NotNull(ServiceContext.Provider);
                Assert.Equal(ServiceContext.Provider, provider);
            }
        }
    }
}
