using System;
using LightBDD.Configuration;
using LightBDD.Core.Notification;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Configuration
{
    [TestFixture]
    public class FeatureProgressNotifierConfiguration_tests
    {
        [Test]
        public void Should_initialize_object_with_default_values()
        {
            var configuration = new FeatureProgressNotifierConfiguration();
            Assert.That(configuration.Notifier, Is.InstanceOf<NoProgressNotifier>());
        }

        [Test]
        public void It_should_not_allow_null_notifier()
        {
            Assert.Throws<ArgumentNullException>(() => new FeatureProgressNotifierConfiguration().UpdateNotifier(null));
        }

        [Test]
        public void It_should_update_configuration()
        {
            var configuration = new FeatureProgressNotifierConfiguration().UpdateNotifier(new DelegatingFeatureProgressNotifier());
            Assert.That(configuration.Notifier, Is.InstanceOf<DelegatingFeatureProgressNotifier>());
        }
    }
}