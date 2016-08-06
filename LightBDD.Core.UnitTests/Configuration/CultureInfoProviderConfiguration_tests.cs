using System;
using LightBDD.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.Core.UnitTests.Configuration
{
    [TestFixture]
    public class CultureInfoProviderConfiguration_tests
    {
        [Test]
        public void It_should_return_default_value()
        {
            Assert.That(new CultureInfoProviderConfiguration().CultureInfoProvider, Is.InstanceOf<DefaultCultureInfoProvider>());
        }

        [Test]
        public void It_should_not_allow_null_value()
        {
            Assert.Throws<ArgumentNullException>(() => new CultureInfoProviderConfiguration().UpdateCultureInfoProvider(null));
        }

        [Test]
        public void It_should_update_value()
        {
            var provider = MockRepository.GenerateMock<ICultureInfoProvider>();
            var config = new CultureInfoProviderConfiguration().UpdateCultureInfoProvider(provider);
            Assert.That(config.CultureInfoProvider, Is.SameAs(provider));
        }
    }
}