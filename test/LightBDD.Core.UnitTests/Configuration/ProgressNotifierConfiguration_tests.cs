#nullable enable
using System;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Notification;
using LightBDD.Core.Notification.Events;
using Moq;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Configuration
{
    [TestFixture]
    public class ProgressNotifierConfiguration_tests
    {
        [Test]
        public void Should_initialize_object_with_default_values()
        {
            var configuration = new ProgressNotifierConfiguration();
            Assert.That(configuration.Notifiers, Is.Empty);
        }

        [Test]
        public void Clear_should_reset_Notifiers()
        {
            var configuration = new ProgressNotifierConfiguration()
                .Register(c => c.Use(Mock.Of<IProgressNotifier>()))
                .Clear();
            Assert.That(configuration.Notifiers, Is.Empty);
        }

        [Test]
        public async Task Append_should_append_notifiers_to_existing_ones()
        {
            var notifier1 = Mock.Of<IProgressNotifier>();
            var notifier2 = Mock.Of<IProgressNotifier>();
            var notifier3 = Mock.Of<IProgressNotifier>();
            var notifier4 = Mock.Of<IProgressNotifier>();

            var cfg = new LightBddConfiguration();
            cfg.ProgressNotifierConfiguration()
                .Register(c => c.Use(notifier1))
                .Register(c => c.Use(notifier2))
                .Register(c => c.Use(notifier3))
                .Register(c => c.Use(notifier4));

            var progressEvent = new ProgressEvent(default);
            await using var container = cfg.BuildContainer();
            container.Resolve<ProgressNotificationDispatcher>().Notify(progressEvent);

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

            Assert.Throws<InvalidOperationException>(() => cfg.Register(c=>c.Use<IProgressNotifier>()));
            Assert.Throws<InvalidOperationException>(() => cfg.Clear());
        }
    }
}