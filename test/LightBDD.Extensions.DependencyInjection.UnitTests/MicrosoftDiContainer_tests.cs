using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.UnitTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace LightBDD.Extensions.DependencyInjection.UnitTests
{
    [TestFixture]
    public class MicrosoftDiContainer_tests : ContainerBaseTests
    {
        protected override IDependencyContainer CreateContainer()
        {
            var serviceCollection = new ServiceCollection()
                .AddTransient<Disposable>();

            return new DependencyContainerConfiguration()
                .UseContainer(serviceCollection.BuildServiceProvider())
                .DependencyContainer;
        }
    }
}
