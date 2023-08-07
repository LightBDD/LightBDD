using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.UnitTests.Scenarios.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Framework.Scenarios;

namespace LightBDD.Framework.UnitTests.Scenarios.Contextual
{
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class Contextual_runner_context_disposal_tests
    {
        private readonly CapturingContextBuilder _builder = new();
        private readonly IBddRunner _runner;

        public Contextual_runner_context_disposal_tests()
        {
            _runner = ScenarioMocks.CreateBddRunner(new[] { _builder });
        }

        [Test]
        public void WithContext_accepting_instance_should_not_takeOwnership_by_default()
        {
            var expected = new Testable();
            _runner.WithContext(expected);
            AssertRegistration(expected, false);
        }

        [Test]
        public void WithContext_accepting_instance_should_honor_takeOwnership_override()
        {
            var expected = new Testable();
            _runner.WithContext(expected, true);
            AssertRegistration(expected, true);
        }

        [Test]
        public void WithContext_accepting_instance_factory_should_takeOwnership_by_default()
        {
            var expected = new Testable();
            _runner.WithContext(() => expected);

            AssertRegistration(expected, true);
        }

        [Test]
        public void WithContext_accepting_instance_factory_should_honor_takeOwnership_override()
        {
            var expected = new Testable();
            _runner.WithContext(() => expected, false);

            AssertRegistration(expected, false);
        }

        [Test]
        public void Generic_WithContext_should_use_DI_container()
        {
            _runner.WithContext<Testable>();

            AssertRegistration(null, true);
        }

        private void AssertRegistration(Testable instance, bool shouldTakeOwnership)
        {
            var container = new DependencyContainerConfiguration().DependencyContainer;
            Testable actual;
            using (var scope = container.BeginScope(LifetimeScope.Local))
            {
                actual = (Testable)_builder.Descriptor.ContextResolver(scope);
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

        class CapturingContextBuilder : ICoreScenarioStepsRunner
        {
            public ExecutionContextDescriptor Descriptor { get; private set; }

            public ICoreScenarioStepsRunner AddSteps(IEnumerable<StepDescriptor> steps) => throw new NotImplementedException();

            public ICoreScenarioStepsRunner WithContext(ExecutionContextDescriptor contextDescriptor)
            {
                Descriptor = contextDescriptor;
                return this;
            }

            public Task RunAsync() => throw new NotImplementedException();

            public LightBddConfiguration Configuration => throw new NotImplementedException();
        }
    }
}
