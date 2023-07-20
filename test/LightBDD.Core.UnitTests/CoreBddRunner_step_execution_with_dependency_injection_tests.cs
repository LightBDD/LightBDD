using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using LightBDD.ScenarioHelpers;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using Moq;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    public class CoreBddRunner_step_execution_with_dependency_injection_tests
    {
        private IBddRunner _runner;
        private IFeatureRunner _feature;
        private  Mock<IDependencyContainerV2> _containerScope;
        private Mock<IDependencyContainerV2> _scenarioScope;
        private Mock<IDependencyContainerV2> _stepScope;

        [SetUp]
        public void SetUp()
        {
            _containerScope = new Mock<IDependencyContainerV2>();
            _scenarioScope = new Mock<IDependencyContainerV2>();
            _stepScope = new Mock<IDependencyContainerV2>();

            _containerScope.Setup(x => x.BeginScope(It.IsAny<LifetimeScope>(), It.IsAny<Action<ContainerConfigurator>>()))
                .Returns(_scenarioScope.Object);

            _scenarioScope.Setup(x => x.BeginScope(It.IsAny<LifetimeScope>(), It.IsAny<Action<ContainerConfigurator>>()))
                .Returns(_stepScope.Object);

            _feature = new TestableFeatureRunnerRepository(TestableIntegrationContextBuilder.Default()
                    .WithConfiguration(c => c.DependencyContainerConfiguration().UseContainer(_containerScope.Object)))
                .GetRunnerFor(GetType());

            _runner = _feature.GetBddRunner(this);
        }

        [Test]
        public void Runner_should_instantiate_scenario_context_within_scenario_scope()
        {
            _runner.Test()
                .WithContext(r => r.Resolve<MyScenarioScope>())
                .TestScenario(Given_step_one);

            _containerScope.Verify(s => s.BeginScope(LifetimeScope.Scenario, It.IsAny<Action<ContainerConfigurator>>()));
            _scenarioScope.Verify(s => s.Resolve(typeof(MyScenarioScope)));
        }

        [Test]
        public void Runner_should_instantiate_step_context_within_step_scope()
        {
            _runner.Test()
                .WithContext(r => r.Resolve<MyScenarioScope>())
                .TestGroupScenario(Given_composite, Given_composite);

            _containerScope.Verify(s => s.BeginScope(LifetimeScope.Scenario, It.IsAny<Action<ContainerConfigurator>>()), Times.Once);
            _scenarioScope.Verify(s => s.BeginScope(LifetimeScope.Local, It.IsAny<Action<ContainerConfigurator>>()), Times.Exactly(2));
            _stepScope.Verify(s => s.Resolve(typeof(MyStepScope)), Times.Exactly(2));
        }

        class MyScenarioScope { }
        class MyStepScope { }

        private void Given_step_one() { }
        private TestCompositeStep Given_composite()
        {
            return new TestCompositeStep(
                new ExecutionContextDescriptor(r => r.Resolve<MyStepScope>(), null),
                TestStep.CreateSync(Given_step_one));
        }
    }
}