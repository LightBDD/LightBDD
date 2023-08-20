#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Notification;
using LightBDD.Core.Notification.Events;
using Microsoft.Extensions.DependencyInjection;
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
            var configuration = new LightBddConfiguration();
            Assert.That(GetDescriptors(configuration), Is.Empty);
        }

        [Test]
        public void Clear_should_reset_Notifiers()
        {
            var configuration = new LightBddConfiguration();

            configuration.Services.ConfigureProgressNotifiers()
                .Add(Mock.Of<IProgressNotifier>())
                .Clear();

            Assert.That(GetDescriptors(configuration), Is.Empty);
        }

        [Test]
        public async Task Append_should_append_notifiers_to_existing_ones()
        {
            var notifier1 = Mock.Of<IProgressNotifier>();
            var notifier2 = Mock.Of<IProgressNotifier>();
            var notifier3 = Mock.Of<IProgressNotifier>();
            var notifier4 = Mock.Of<IProgressNotifier>();

            var cfg = new LightBddConfiguration();
            cfg.Services.ConfigureProgressNotifiers()
                .Add(notifier1)
                .Add(notifier2)
                .Add(notifier3)
                .Add(notifier4);

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
            var cfg = root.Services.ConfigureProgressNotifiers()
                .Add(Mock.Of<IProgressNotifier>());
            root.Seal();

            Assert.Throws<InvalidOperationException>(() => cfg.Add<IProgressNotifier>());
            Assert.Throws<InvalidOperationException>(() => cfg.Clear());
        }

        private static IEnumerable<ServiceDescriptor> GetDescriptors(LightBddConfiguration configuration)
        {
            return configuration.Services.Where(x => x.ServiceType == typeof(IProgressNotifier));
        }
    }
}