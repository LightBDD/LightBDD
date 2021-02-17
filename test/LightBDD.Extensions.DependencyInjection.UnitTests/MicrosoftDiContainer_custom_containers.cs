using System;
using LightBDD.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace LightBDD.Extensions.DependencyInjection.UnitTests
{
    [TestFixture]
    public class MicrosoftDiContainer_custom_containers
    {
        [Test]
        public void UseContainer_requires_disposable_providers_if_ownership_is_expected()
        {
            var serviceProvider = new FakeNonDisposableProvider();

            var ex = Assert.Throws<ArgumentException>(() => new DependencyContainerConfiguration().UseContainer(serviceProvider, true));
            Assert.That(ex.Message,
                Does.Contain($"The provided {typeof(FakeNonDisposableProvider).FullName} is not IDisposable and LightBDD cannot take proper ownership of the provider. Please consider disabling takeOwnership configuration flag and manual disposal of the provider."));
        }

        [Test]
        public void UseContainer_allows_non_disposable_providers_as_long_as_LightBDD_does_not_take_ownership()
        {
            var serviceProvider = new FakeNonDisposableProvider();
            Assert.DoesNotThrow(() => new DependencyContainerConfiguration().UseContainer(serviceProvider, false));
        }

        class FakeNonDisposableProvider : IServiceProvider
        {
            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(IServiceScopeFactory))
                    return Mock.Of<IServiceScopeFactory>();
                throw new NotImplementedException();
            }
        }
    }
}