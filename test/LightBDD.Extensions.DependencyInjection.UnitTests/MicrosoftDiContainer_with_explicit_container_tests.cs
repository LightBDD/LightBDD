using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.UnitTests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace LightBDD.Extensions.DependencyInjection.UnitTests
{
    [TestFixture(true)]
    [TestFixture(false)]
    public class MicrosoftDiContainer_with_explicit_container_tests : ContainerBaseTests
    {
        public MicrosoftDiContainer_with_explicit_container_tests(bool shouldTakeOwnership)
            : base(shouldTakeOwnership)
        {
        }
        protected override IDependencyContainer CreateContainer()
        {
            var serviceCollection = new ServiceCollection()
                .AddTransient<Disposable>()
                .AddSingleton<DisposableSingleton>();

            return new DependencyContainerConfiguration()
                .UseContainer(serviceCollection.BuildServiceProvider(), ShouldTakeOwnership)
                .DependencyContainer;
        }
    }
}