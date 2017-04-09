using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Formatting;
using LightBDD.Core.Notification;
using LightBDD.Framework.Notification;
using LightBDD.Framework.Notification.Configuration;
using Moq;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Notification.Configuration
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

        [Test]
        public void Configuration_should_be_sealable()
        {
            var lighbddConfig = new LightBddConfiguration();
            var cfg = lighbddConfig.Get<FeatureProgressNotifierConfiguration>();
            lighbddConfig.Seal();

            Assert.Throws<InvalidOperationException>(() => cfg.UpdateNotifier(Mock.Of<IFeatureProgressNotifier>()));
        }
    }
}