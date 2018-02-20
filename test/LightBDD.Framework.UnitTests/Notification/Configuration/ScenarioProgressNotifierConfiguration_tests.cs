using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Framework.Notification;
using LightBDD.Framework.Notification.Configuration;
using Moq;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Notification.Configuration
{
    [TestFixture]
    public class ScenarioProgressNotifierConfiguration_tests
    {
        [Test]
        public void Should_initialize_object_with_default_values()
        {
            var configuration = new ScenarioProgressNotifierConfiguration();
            Assert.That(configuration.NotifierProvider, Is.Not.Null);
            Assert.That(configuration.NotifierProvider(null), Is.InstanceOf<NoProgressNotifier>());
        }

        [Test]
        public void UpdateNotifierProvider_should_not_allow_null_notifier_provider()
        {
            Assert.Throws<ArgumentNullException>(() => new ScenarioProgressNotifierConfiguration().UpdateNotifierProvider(null));
        }

        [Test]
        public void UpdateNotifierProvider_should_update_configuration()
        {
            var notifier = Mock.Of<IScenarioProgressNotifier>();
            var configuration = new ScenarioProgressNotifierConfiguration().UpdateNotifierProvider(() => notifier);
            Assert.That(configuration.NotifierProvider(null), Is.SameAs(notifier));
        }

        [Test]
        public void UpdateNotifierProvider_should_update_configuration_with_fixture_object()
        {
            var notifier = Mock.Of<IScenarioProgressNotifier>();
            object capturedFixture = null;
            var configuration = new ScenarioProgressNotifierConfiguration().UpdateNotifierProvider<object>(fixture =>
            {
                capturedFixture = fixture;
                return notifier;
            });

            var expectedFixture = new object();
            Assert.That(configuration.NotifierProvider(expectedFixture), Is.SameAs(notifier));
            Assert.That(capturedFixture, Is.SameAs(expectedFixture));
        }

        [Test]
        public void Configured_provider_should_throw_meaningful_exception_if_fixture_type_does_not_match()
        {
            var configuration = new ScenarioProgressNotifierConfiguration().UpdateNotifierProvider<ScenarioProgressNotifierConfiguration_tests>(fixture => NoProgressNotifier.Default);

            var ex = Assert.Throws<InvalidOperationException>(() => configuration.NotifierProvider(new object()));
            Assert.That(ex.Message, Is.EqualTo($"Unable to create {nameof(IScenarioProgressNotifier)}. Expected fixture of type '{GetType()}' while got '{typeof(object)}'."));
        }

        [Test]
        public void Configured_provider_should_throw_exception_if_fixture_is_null()
        {
            var configuration = new ScenarioProgressNotifierConfiguration().UpdateNotifierProvider<ScenarioProgressNotifierConfiguration_tests>(fixture => NoProgressNotifier.Default);

            Assert.Throws<ArgumentNullException>(() => configuration.NotifierProvider(null));
        }

        [Test]
        public void ClearNotifierProviders_should_reset_it_to_NoProgressNotifier()
        {
            var configuration = new ScenarioProgressNotifierConfiguration()
                .UpdateNotifierProvider(Mock.Of<IScenarioProgressNotifier>)
                .ClearNotifierProviders();
            Assert.That(configuration.NotifierProvider(null), Is.InstanceOf<NoProgressNotifier>());
        }

        [Test]
        public void AppendNotifierProviders_should_append_notifiers_to_existing_ones()
        {
            var notifier1 = Mock.Of<IScenarioProgressNotifier>();
            var notifier2 = Mock.Of<IScenarioProgressNotifier>();
            var notifier3 = Mock.Of<IScenarioProgressNotifier>();
            var notifier4 = Mock.Of<IScenarioProgressNotifier>();

            var configuration = new ScenarioProgressNotifierConfiguration()
                .AppendNotifierProviders(() => notifier1, () => notifier2)
                .AppendNotifierProviders<object>(fixture => notifier3, fixture => notifier4);

            var notifier = configuration.NotifierProvider(new object());

            Assert.That(notifier, Is.InstanceOf<DelegatingScenarioProgressNotifier>());
            Assert.That(((DelegatingScenarioProgressNotifier)notifier).Notifiers, Is.EqualTo(new[] { notifier1, notifier2, notifier3, notifier4 }));
        }

        [Test]
        public void UpdateNotifierProvider_should_not_throw_StackOverflowException_when_added_self()
        {
            var notifier1 = Mock.Of<IScenarioProgressNotifier>();
            var notifier2 = Mock.Of<IScenarioProgressNotifier>();
            var configuration = new ScenarioProgressNotifierConfiguration();
            configuration.UpdateNotifierProvider(() => notifier1);

            var previous = configuration.NotifierProvider;

            configuration
                .UpdateNotifierProvider<object>(feature => new DelegatingScenarioProgressNotifier(previous(feature), notifier2));

            var notifier = configuration.NotifierProvider(new object());
            notifier.NotifyScenarioStart(Mock.Of<IScenarioInfo>());

            Mock.Get(notifier1).Verify(x => x.NotifyScenarioStart(It.IsAny<IScenarioInfo>()), Times.Once);
            Mock.Get(notifier2).Verify(x => x.NotifyScenarioStart(It.IsAny<IScenarioInfo>()), Times.Once);
        }

        [Test]
        public void AppendNotifierProvider_should_not_throw_StackOverflowException_when_added_self()
        {
            var notifier1 = Mock.Of<IScenarioProgressNotifier>();
            var configuration = new ScenarioProgressNotifierConfiguration();
            configuration.UpdateNotifierProvider(() => notifier1);

            configuration
                .AppendNotifierProviders(configuration.NotifierProvider);

            var notifier = configuration.NotifierProvider(new object());
            notifier.NotifyScenarioStart(Mock.Of<IScenarioInfo>());

            Mock.Get(notifier1).Verify(x => x.NotifyScenarioStart(It.IsAny<IScenarioInfo>()), Times.Exactly(2));
        }

        [Test]
        public void Configuration_should_be_sealable()
        {
            var lighbddConfig = new LightBddConfiguration();
            var cfg = lighbddConfig.Get<ScenarioProgressNotifierConfiguration>();
            lighbddConfig.Seal();

            Assert.Throws<InvalidOperationException>(() => cfg.UpdateNotifierProvider(Mock.Of<IScenarioProgressNotifier>));
            Assert.Throws<InvalidOperationException>(() => cfg.UpdateNotifierProvider((object fixture) => Mock.Of<IScenarioProgressNotifier>()));
            Assert.Throws<InvalidOperationException>(() => cfg.AppendNotifierProviders((object fixture) => Mock.Of<IScenarioProgressNotifier>()));
            Assert.Throws<InvalidOperationException>(() => cfg.AppendNotifierProviders(Mock.Of<IScenarioProgressNotifier>));
            Assert.Throws<InvalidOperationException>(() => cfg.ClearNotifierProviders());
        }
    }
}