using System;
using System.Linq;
using LightBDD.Core.Extensibility;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Extensibility
{
    [TestFixture]
    public class BddRunnerFactory_tests
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
            var ex = Assert.Throws<ArgumentNullException>(() => _factory.GetRunnerFor(null));
            Assert.That(ex.Message, Does.Contain("featureType"));
        }

        [Test]
        public void It_should_throw_if_runner_requested_with_null_fixture()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _factory.GetRunnerFor(GetType()).GetRunner(null));
            Assert.That(ex.Message, Does.Contain("fixture"));
        }

        [Test]
        public void It_should_throw_if_runner_requested_with_fixture_of_different_type()
        {
            var ex = Assert.Throws<ArgumentException>(() => _factory.GetRunnerFor(GetType()).GetRunner(new object()));
            Assert.That(ex.Message, Is.EqualTo($"Provided fixture instance '{typeof(object)}' type does not match feature type '{GetType()}'"));
        }

        [Test]
        public void It_should_instantiate_only_one_runner_per_type()
        {
            var runner1 = _factory.GetRunnerFor(GetType());
            var runner2 = _factory.GetRunnerFor(GetType());
            Assert.That(runner1, Is.SameAs(runner2));
        }

        [Test]
        public void It_should_return_all_runners()
        {
            _factory.GetRunnerFor(GetType());
            _factory.GetRunnerFor(typeof(BddRunnerExtensions_tests));
            Assert.That(_factory.AllRunners.Count(), Is.EqualTo(2));
        }
    }
}