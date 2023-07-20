using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Notification;
using LightBDD.Core.Notification.Events;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Configuration
{
    [TestFixture]
    public class ProgressNotifierConfiguration_tests
    {
        [Test]
        public void Should_initialize_object_with_default_values()
        {
            var configuration = new ProgressNotifierConfiguration();
            Assert.That(configuration.Notifier, Is.InstanceOf<NoProgressNotifier>());
        }

        [Test]
        public void Append_should_not_allow_null_notifier()
        {
            Assert.Throws<ArgumentNullException>(() => new ProgressNotifierConfiguration().Append(null!))
                ?.Message.ShouldContain("notifiers");

            Assert.Throws<ArgumentNullException>(() => new ProgressNotifierConfiguration().Append(((IProgressNotifier)null)!))
                ?.Message.ShouldContain("Value notifiers[0] cannot be null");
        }

        [Test]
        public void Clear_should_reset_it_to_NoProgressNotifier()
        {
            var configuration = new ProgressNotifierConfiguration()
                .Append(Mock.Of<IProgressNotifier>())
                .Clear();
            Assert.That(configuration.Notifier, Is.InstanceOf<NoProgressNotifier>());
        }

        [Test]
        public void Append_should_append_notifiers_to_existing_ones()
        {
            var notifier1 = Mock.Of<IProgressNotifier>();
            var notifier2 = Mock.Of<IProgressNotifier>();
            var notifier3 = Mock.Of<IProgressNotifier>();
            var notifier4 = Mock.Of<IProgressNotifier>();

            var configuration = new ProgressNotifierConfiguration()
                .Append(notifier1, notifier2)
                .Append(notifier3)
                .Append(notifier4);

            var progressEvent = new ProgressEvent(default);
            configuration.Notifier.Notify(progressEvent);

            Mock.Get(notifier1).Verify(x => x.Notify(progressEvent));
            Mock.Get(notifier2).Verify(x => x.Notify(progressEvent));
            Mock.Get(notifier3).Verify(x => x.Notify(progressEvent));
            Mock.Get(notifier4).Verify(x => x.Notify(progressEvent));
        }

        [Test]
        public void Configuration_should_be_sealable()
        {
            var root = new LightBddConfiguration();
            var cfg = root.Get<ProgressNotifierConfiguration>();
            root.Seal();

            Assert.Throws<InvalidOperationException>(() => cfg.Append(Mock.Of<IProgressNotifier>()));
            Assert.Throws<InvalidOperationException>(() => cfg.Clear());
        }
    }
}