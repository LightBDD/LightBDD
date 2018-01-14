using System;
using LightBDD.Core.Configuration;
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
        public void UpdateNotifier_should_not_allow_null_notifier()
        {
            Assert.Throws<ArgumentNullException>(() => new FeatureProgressNotifierConfiguration().UpdateNotifier(null));
        }

        [Test]
        public void UpdateNotifier_should_update_configuration()
        {
            var configuration = new FeatureProgressNotifierConfiguration().UpdateNotifier(new DelegatingFeatureProgressNotifier());
            Assert.That(configuration.Notifier, Is.InstanceOf<DelegatingFeatureProgressNotifier>());
        }

        [Test]
        public void ClearNotifiers_should_reset_it_to_NoProgressNotifier()
        {
            var configuration = new FeatureProgressNotifierConfiguration()
                .UpdateNotifier(new DelegatingFeatureProgressNotifier())
                .ClearNotifiers();
            Assert.That(configuration.Notifier, Is.InstanceOf<NoProgressNotifier>());
        }

        [Test]
        public void AppendNotifiers_should_append_notifiers_to_existing_ones()
        {
            var notifier1 = Mock.Of<IFeatureProgressNotifier>();
            var notifier2 = Mock.Of<IFeatureProgressNotifier>();
            var notifier3 = Mock.Of<IFeatureProgressNotifier>();
            var notifier4 = Mock.Of<IFeatureProgressNotifier>();

            var configuration = new FeatureProgressNotifierConfiguration()
                .AppendNotifiers(notifier1, notifier2)
                .AppendNotifiers(notifier3)
                .AppendNotifiers(notifier4);
            Assert.That(configuration.Notifier, Is.InstanceOf<DelegatingFeatureProgressNotifier>());
            Assert.That(((DelegatingFeatureProgressNotifier)configuration.Notifier).Notifiers, Is.EqualTo(new[] { notifier1, notifier2, notifier3, notifier4 }));
        }

        [Test]
        public void Configuration_should_be_sealable()
        {
            var root = new LightBddConfiguration();
            var cfg = root.Get<FeatureProgressNotifierConfiguration>();
            root.Seal();

            Assert.Throws<InvalidOperationException>(() => cfg.UpdateNotifier(Mock.Of<IFeatureProgressNotifier>()));
            Assert.Throws<InvalidOperationException>(() => cfg.AppendNotifiers(Mock.Of<IFeatureProgressNotifier>()));
            Assert.Throws<InvalidOperationException>(() => cfg.ClearNotifiers());
        }
    }
}