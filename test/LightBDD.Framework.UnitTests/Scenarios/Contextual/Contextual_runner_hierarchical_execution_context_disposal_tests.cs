using System;
using LightBDD.Core.Configuration;
using LightBDD.Framework.Scenarios;
using LightBDD.Framework.UnitTests.Scenarios.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Scenarios.Contextual
{
    [TestFixture]
    public class Contextual_runner_hierarchical_execution_context_disposal_tests
    {
        private ICompositeStepBuilder _builder;

        [SetUp]
        public void SetUp()
        {
            _builder = new TestableCompositeStepBuilder();
        }

        [Test]
        public void WithContext_accepting_instance_should_not_takeOwnership_by_default()
        {
            var instance = new Testable();
            var step = _builder
                 .WithContext(instance)
                 .Build();
            AssertRegistration(instance, step, false);
        }

        [Test]
        public void WithContext_accepting_instance_should_honor_takeOwnership_override()
        {
            var instance = new Testable();
            var step = _builder
                .WithContext(instance, true)
                .Build();
            AssertRegistration(instance, step, true);
        }

        [Test]
        public void WithContext_accepting_instance_factory_should_takeOwnership_by_default()
        {
            var instance = new Testable();
            var step = _builder
                .WithContext(() => instance)
                .Build();
            AssertRegistration(instance, step, true);
        }

        [Test]
        public void WithContext_accepting_instance_factory_should_honor_takeOwnership_override()
        {
            var instance = new Testable();
            var step = _builder
                .WithContext(() => instance, false)
                .Build();
            AssertRegistration(instance, step, false);
        }

        [Test]
        public void Generic_WithContext_should_takeOwnership_by_default()
        {
            var step = _builder
                .WithContext<Testable>()
                .Build();
            AssertRegistration(null, step, true);
        }

        private void AssertRegistration(Testable instance, CompositeStep step, bool shouldTakeOwnership)
        {
            using var container = new DependencyContainerConfiguration().Build();
            Testable actual;
            using (var scope = container.BeginScope())
            {
                actual = (Testable)step.SubStepsContext.ContextResolver(scope);
                if (instance != null)
                    Assert.That(actual, Is.SameAs(instance));
            }
            Assert.That(actual.Disposed, Is.EqualTo(shouldTakeOwnership));
        }

        class Testable : IDisposable
        {
            public void Dispose()
            {
                Disposed = true;
            }

            public bool Disposed { get; set; }
        }
    }
}
