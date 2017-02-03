using System;
using LightBDD.Core.Notification;
using LightBDD.Framework.Notification;
using LightBDD.Framework.Notification.Configuration;
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
        public void It_should_not_allow_null_notifier_provider()
        {
            Assert.Throws<ArgumentNullException>(() => new ScenarioProgressNotifierConfiguration().UpdateNotifierProvider(null));
        }

        [Test]
        public void It_should_update_configuration()
        {
            var configuration = new ScenarioProgressNotifierConfiguration().UpdateNotifierProvider(() => new DelegatingScenarioProgressNotifier());
            Assert.That(configuration.NotifierProvider(null), Is.InstanceOf<DelegatingScenarioProgressNotifier>());
        }

        [Test]
        public void It_should_update_configuration_with_fixture_object()
        {
            object capturedFixture = null;
            var configuration = new ScenarioProgressNotifierConfiguration().UpdateNotifierProvider<object>(fixture =>
            {
                capturedFixture = fixture;
                return new DelegatingScenarioProgressNotifier();
            });

            var expectedFixture = new object();
            Assert.That(configuration.NotifierProvider(expectedFixture), Is.InstanceOf<DelegatingScenarioProgressNotifier>());
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
    }
}