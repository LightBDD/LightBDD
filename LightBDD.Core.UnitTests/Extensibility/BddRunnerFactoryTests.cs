using System;
using System.Linq;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Notification;
using LightBDD.Core.UnitTests.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Extensibility
{
    [TestFixture]
    public class BddRunnerFactoryTests
    {
        private BddRunnerFactory _factory;

        [SetUp]
        public void SetUp()
        {
            _factory = new TestableBddRunnerFactory();
        }

        [Test]
        public void It_should_throw_if_runner_requested_with_null_type_parameter()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _factory.GetRunnerFor(null, () => new NoProgressNotifier()));
            Assert.That(ex.Message, Does.Contain("featureType"));
        }

        [Test]
        public void It_should_throw_if_runner_requested_with_null_progress_notifier()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _factory.GetRunnerFor(GetType(), null));
            Assert.That(ex.Message, Does.Contain("progressNotifierProvider"));
        }

        [Test]
        public void It_should_instantiate_only_one_runner_per_type()
        {
            var runner1 = _factory.GetRunnerFor(GetType(), () => new NoProgressNotifier());
            var runner2 = _factory.GetRunnerFor(GetType(), () => new NoProgressNotifier());
            Assert.That(runner1, Is.SameAs(runner2));
        }

        //TODO: This should be really revisited and either allow to use different progress notifiers or not allow to change it at all
        [Test]
        public void It_should_use_progress_notifier_only_on_first_instantiation()
        {
            _factory.GetRunnerFor(GetType(), () => new NoProgressNotifier());
            var runner = _factory.GetRunnerFor(GetType(), () => new DelegatingProgressNotifier());
            Assert.That(runner.IntegrationContext.ProgressNotifier, Is.InstanceOf<NoProgressNotifier>());
        }

        [Test]
        public void It_should_return_all_runners()
        {
            _factory.GetRunnerFor(GetType(), () => new NoProgressNotifier());
            _factory.GetRunnerFor(typeof(BddRunnerExtensionsTests), () => new NoProgressNotifier());
            Assert.That(_factory.AllRunners.Count(), Is.EqualTo(2));
        }
    }
}