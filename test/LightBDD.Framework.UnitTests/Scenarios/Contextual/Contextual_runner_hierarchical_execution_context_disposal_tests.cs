using System;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Framework.Scenarios;
using LightBDD.Framework.UnitTests.Scenarios.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Scenarios.Contextual
{
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class Contextual_runner_hierarchical_execution_context_disposal_tests
    {
        private readonly ICompositeStepBuilder _builder = new TestableCompositeStepBuilder();

        [Test]
        public async Task WithContext_accepting_instance_should_not_takeOwnership_by_default()
        {
            var instance = new Testable();
            var step = _builder
                 .WithContext(instance)
                 .Build();
            await AssertRegistration(instance, step, false);
        }

        [Test]
        public async Task WithContext_accepting_instance_should_honor_takeOwnership_override()
        {
            var instance = new Testable();
            var step = _builder
                .WithContext(instance, true)
                .Build();
            await AssertRegistration(instance, step, true);
        }

        [Test]
        public async Task WithContext_accepting_instance_factory_should_takeOwnership_by_default()
        {
            var instance = new Testable();
            var step = _builder
                .WithContext(() => instance)
                .Build();
            await AssertRegistration(instance, step, true);
        }

        [Test]
        public async Task WithContext_accepting_instance_factory_should_honor_takeOwnership_override()
        {
            var instance = new Testable();
            var step = _builder
                .WithContext(() => instance, false)
                .Build();
            await AssertRegistration(instance, step, false);
        }

        [Test]
        public async Task Generic_WithContext_should_takeOwnership_by_default()
        {
            var step = _builder
                .WithContext<Testable>()
                .Build();
            await AssertRegistration(null, step, true);
        }

        private async Task AssertRegistration(Testable instance, CompositeStep step, bool shouldTakeOwnership)
        {
            await using var container = new LightBddConfiguration().BuildContainer();
            Testable actual;
            await using (var scope = container.BeginScope())
            {
                actual = (Testable)step.SubStepsContext.Resolve(scope);
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
