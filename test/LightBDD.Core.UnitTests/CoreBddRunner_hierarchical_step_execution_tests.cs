using System.Linq;
using LightBDD.Core.Extensibility;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    public class CoreBddRunner_hierarchical_step_execution_tests : Steps
    {
        private IBddRunner _runner;
        private IFeatureRunner _feature;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _feature = TestableFeatureRunnerRepository.GetRunner(GetType());
            _runner = _feature.GetBddRunner(this);
        }

        #endregion

        [Test]
        public void Runner_should_execute_all_steps_within_group()
        {
            Assert.DoesNotThrow(() => _runner.Test().TestGroupScenario(Grouped_steps));
            var steps = _feature.GetFeatureResult().GetScenarios().Single().GetSteps().ToArray();
            Assert.That(steps, Has.Length.EqualTo(1));
            Assert.That(steps[0].Info.Name.ToString(), Is.EqualTo("Grouped steps"));

            Assert.That(steps[0].SubSteps, Is.Not.Null);
            Assert.That(
                steps[0].SubSteps.Select(s => s.Info.Name.ToString()).ToArray(),
                Is.EqualTo(new[] { "GIVEN step one", "WHEN step two", "THEN step three" }));
        }

        private StepGroup Grouped_steps()
        {
            return _runner.Test().CreateStepGroup(Given_step_one, When_step_two, Then_step_three);
        }
    }
}