using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.ScenarioHelpers;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Execution
{
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    //TODO: simplify scopes
    public class RunnableScenario_step_execution_with_dependency_injection_tests
    {
        private readonly Mock<IDependencyContainer> _containerScope;
        private readonly Mock<IDependencyContainer> _scenarioScope;
        private readonly TestableScenarioFactory _factory;

        public RunnableScenario_step_execution_with_dependency_injection_tests()
        {
            _containerScope = new Mock<IDependencyContainer>();
            _scenarioScope = new Mock<IDependencyContainer>();

            _containerScope.Setup(x => x.BeginScope())
                .Returns(_scenarioScope.Object);

            _factory = TestableScenarioFactory.Create(cfg => cfg.DependencyContainerConfiguration().UseContainer(_containerScope.Object));
        }

        [Test]
        public async Task Runner_should_instantiate_scenario_context_within_scenario_scope()
        {
            var result = await _factory.RunScenario(x => x.Test()
                .WithContext(r => r.Resolve<MyScenarioScope>())
                .TestScenario(Given_step_one));
            result.Status.ShouldBe(ExecutionStatus.Passed);

            _containerScope.Verify(s => s.BeginScope());
            _scenarioScope.Verify(s => s.Resolve(typeof(MyScenarioScope)));
        }

        [Test]
        public async Task Runner_should_instantiate_step_context_within_scenario_scope()
        {
            var result = await _factory.RunScenario(x => x.Test()
                .WithContext(r => r.Resolve<MyScenarioScope>())
                .TestGroupScenario(Given_composite, Given_composite));
            result.Status.ShouldBe(ExecutionStatus.Passed);

            _containerScope.Verify(s => s.BeginScope(), Times.Once);
            _scenarioScope.Verify(s => s.Resolve(typeof(MyStepScope)), Times.Exactly(2));
        }

        class MyScenarioScope
        {
        }

        class MyStepScope
        {
        }

        private void Given_step_one()
        {
        }

        private TestCompositeStep Given_composite()
        {
            return new TestCompositeStep(
                new ExecutionContextDescriptor(r => r.Resolve<MyStepScope>()),
                TestStep.CreateSync(Given_step_one));
        }
    }
}