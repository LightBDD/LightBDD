using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using Moq;
using NUnit.Framework;

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
            var provider = Mock.Of<ICultureInfoProvider>();
            var config = new CultureInfoProviderConfiguration().UpdateCultureInfoProvider(provider);
            Assert.That(config.CultureInfoProvider, Is.SameAs(provider));
        }
    }
}