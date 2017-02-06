using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Metadata;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Framework.Extensibility;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using Moq;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    public class CoreBddRunner_execution_extension_tests : Steps
    {
        [Test]
        public void It_should_call_scenario_extensions_when_scenario_is_executed()
        {
            var mockExtension = new Mock<IScenarioExecutionExtension>();
            mockExtension
                .Setup(e => e.ExecuteAsync(It.IsAny<IScenarioInfo>(), It.IsAny<Func<Task>>()))
                .Returns((IScenarioInfo info, Func<Task> inner) => inner());

            var featureRunner = CreateRunner(new ExecutionExtensionsConfiguration().EnableScenarioExtension(() => mockExtension.Object));
            var runner = featureRunner.GetBddRunner(this);

            runner
                .Test()
                .TestScenario(Some_step);

            var scenarios = featureRunner.GetFeatureResult().GetScenarios().ToArray();
            Assert.That(scenarios.Length, Is.EqualTo(1));

            mockExtension.Verify(e => e.ExecuteAsync(scenarios[0].Info, It.IsAny<Func<Task>>()), Times.Once);
        }

        [Test]
        public void It_should_call_step_extensions_when_scenario_is_executed()
        {
            var mockExtension = new Mock<IStepExecutionExtension>();
            mockExtension
                .Setup(e => e.ExecuteAsync(It.IsAny<IStep>(), It.IsAny<Func<Task>>()))
                .Returns((IStep step, Func<Task> inner) =>
                {
                    step.Comment(step.Info.Name.ToString());
                    return inner();
                });

            var featureRunner = CreateRunner(new ExecutionExtensionsConfiguration().EnableStepExtension(() => mockExtension.Object));
            var runner = featureRunner.GetBddRunner(this);

            runner
                .Test()
                .TestScenario(
                    Given_step_one,
                    When_step_two,
                    Then_step_three);

            mockExtension.Verify(e => e.ExecuteAsync(It.IsAny<IStep>(), It.IsAny<Func<Task>>()), Times.Exactly(3));

            var steps = featureRunner.GetFeatureResult().GetScenarios().Single().GetSteps().ToArray();
            Assert.That(steps.Length, Is.EqualTo(3));

            Assert.That(steps[0].Comments, Is.EquivalentTo(new[] { "GIVEN step one" }));
            Assert.That(steps[1].Comments, Is.EquivalentTo(new[] { "WHEN step two" }));
            Assert.That(steps[2].Comments, Is.EquivalentTo(new[] { "THEN step three" }));
        }

        private IFeatureRunner CreateRunner(IExecutionExtensions extensions)
        {
            return new TestableFeatureRunnerRepository(TestableIntegrationContextBuilder.Default().WithExecutionExtensions(extensions)).GetRunnerFor(GetType());
        }
    }


}
