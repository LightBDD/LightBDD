using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.UnitTests.Scenarios.Helpers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Framework.Scenarios;
using Shouldly;

namespace LightBDD.Framework.UnitTests.Scenarios.Contextual
{
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class Contextual_runner_context_disposal_tests
    {
        private readonly CapturingContextBuilder _builder = new();
        private readonly IBddRunner _runner;
        private readonly Mock<ContainerConfigurator> _configurator = new();

        public Contextual_runner_context_disposal_tests()
        {
            _runner = ScenarioMocks.CreateBddRunner(new[] { _builder });
        }

        [Test]
        public void WithContext_accepting_instance_should_not_takeOwnership_by_default()
        {
            _runner.WithContext(new object());

            VerifyExternalOwnership(true);
        }

        [Test]
        public void WithContext_accepting_instance_should_honor_takeOwnership_override()
        {
            _runner.WithContext(new object(), true);

            VerifyExternalOwnership(false);
        }

        [Test]
        public void WithContext_accepting_instance_factory_should_takeOwnership_by_default()
        {
            _runner.WithContext(() => new object());

            VerifyExternalOwnership(false);
        }

        [Test]
        public void WithContext_accepting_instance_factory_should_honor_takeOwnership_override()
        {
            _runner.WithContext(() => new object(), false);

            VerifyExternalOwnership(true);
        }

        [Test]
        public void Generic_WithContext_should_use_DI_container()
        {
            _runner.WithContext<List<string>>();
            _builder.Descriptor.ScopeConfigurator.ShouldBeNull();

            var expected = new List<string>();
            var resolver = new Mock<IDependencyResolver>();
            resolver.Setup(x => x.Resolve(typeof(List<string>))).Returns(expected);

            var actual = _builder.Descriptor.ContextResolver.Invoke(resolver.Object);

            actual.ShouldBeSameAs(expected);
        }

        private void VerifyExternalOwnership(bool expectedExternalOwnership)
        {
            _builder.Descriptor.ScopeConfigurator?.Invoke(_configurator.Object);
            _configurator.Verify(c =>
                c.RegisterInstance(It.IsAny<object>(), It.Is<RegistrationOptions>(x => VerifyExternalOwnership(expectedExternalOwnership, x))));
        }

        private static bool VerifyExternalOwnership(bool expectedExternalOwnership, RegistrationOptions x)
        {
            x.IsExternallyOwned.ShouldBe(expectedExternalOwnership);
            return true;
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
