using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Results;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Scenarios.Fluent;
using Moq;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Scenarios.Fluent
{
    [TestFixture]
    public class Fluent_scenario_tests
    {
        private Mock<IScenarioRunner> _mockScenarioRunner;
        private Mock<ITestableBddRunner> _mockRunner;
        private IBddRunner _runner;

        public interface ITestableBddRunner : IBddRunner, IFeatureFixtureRunner { }

        [SetUp]
        public void SetUp()
        {
            _mockRunner = new Mock<ITestableBddRunner>(MockBehavior.Strict);
            _mockScenarioRunner = new Mock<IScenarioRunner>();
            _runner = _mockRunner.Object;
        }

        [Test]
        public async Task Runner_should_allow_fluent_scenario_execution_in_async_mode()
        {
            var stepDescriptors = new[] { new StepDescriptor("foo", (ctx, args) => Task.FromResult(DefaultStepResultDescriptor.Instance)) };

            ExpectNewScenarioWithSteps(stepDescriptors);
            ExpectRunAsync();

            var builder = _runner.NewScenario();
            builder.Integrate().AddSteps(stepDescriptors);
            await builder.RunAsync();

            VerifyAllExpectations();
        }

        private void VerifyAllExpectations()
        {
            _mockRunner.VerifyAll();
            _mockScenarioRunner.VerifyAll();
        }

        private void ExpectRunAsync()
        {
            _mockScenarioRunner.Setup(x => x.RunScenarioAsync())
                .Returns(Task.FromResult(0))
                .Verifiable();
        }

        private void ExpectNewScenarioWithSteps(StepDescriptor[] stepDescriptors)
        {
            _mockRunner
                .Setup(x => x.NewScenario()).Returns(_mockScenarioRunner.Object)
                .Verifiable();
            _mockScenarioRunner.Setup(x => x.WithCapturedScenarioDetails())
                .Returns(_mockScenarioRunner.Object)
                .Verifiable();
            _mockScenarioRunner.Setup(x => x.WithSteps(stepDescriptors)).Returns(_mockScenarioRunner.Object)
                .Verifiable();
        }
    }
}
