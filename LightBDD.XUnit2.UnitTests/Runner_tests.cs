using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Notification;
using LightBDD.Results;
using Rhino.Mocks;
using Xunit;

namespace LightBDD.XUnit2.UnitTests
{
    [FeatureDescription("desc")]
    [ScenarioCategory("Category D"), ScenarioCategory("Category E")]
    public class Runner_tests
    {
        private readonly IProgressNotifier _progressNotifier;
        private readonly BDDRunner _subject;

        #region Setup/Teardown

        public Runner_tests()
        {
            _progressNotifier = MockRepository.GenerateMock<IProgressNotifier>();
            _subject = new BDDRunner(GetType(), _progressNotifier);
        }

        #endregion

        public void Step_one() { }
        public void Step_throwing_exception() { throw new Exception("some reason"); }
        public void Step_with_inconclusive_assertion() { ScenarioAssert.Ignore("some reason"); }
        public void Step_two() { }

        [Scenario]
        public void Should_collect_scenario_result_for_inconclusive_scenario_steps()
        {
            try
            {
                _subject.RunScenario(Step_one, Step_with_inconclusive_assertion, Step_two);
            }
            catch
            {
            }
            const string expectedStatusDetails = "some reason";

            var result = _subject.Result.Scenarios.Single();
            Assert.Equal("Should collect scenario result for inconclusive scenario steps", result.Name);
            Assert.Equal(ResultStatus.Ignored, result.Status);
            Assert.Equal(3, result.Steps.Count());
            AssertStep(result.Steps, 1, "Step one", ResultStatus.Passed);
            AssertStep(result.Steps, 2, "Step with inconclusive assertion", ResultStatus.Ignored, expectedStatusDetails);
            AssertStep(result.Steps, 3, "Step two", ResultStatus.NotRun);
            Assert.Equal("Step 2: " + expectedStatusDetails, result.StatusDetails);
        }

        [Fact]
        public void Should_collect_scenario_result_for_failing_steps()
        {
            try
            {
                _subject.RunScenario(Step_one, Step_throwing_exception, Step_two);
            }
            catch
            {
            }
            const string expectedStatusDetails = "some reason";

            var result = _subject.Result.Scenarios.Single();
            Assert.Equal("Should collect scenario result for failing steps", result.Name);
            Assert.Equal(ResultStatus.Failed, result.Status);
            Assert.Equal(3, result.Steps.Count());
            AssertStep(result.Steps, 1, "Step one", ResultStatus.Passed);
            AssertStep(result.Steps, 2, "Step throwing exception", ResultStatus.Failed, expectedStatusDetails);
            AssertStep(result.Steps, 3, "Step two", ResultStatus.NotRun);
            Assert.Equal("Step 2: " + expectedStatusDetails, result.StatusDetails);
        }

        [Fact]
        public void Should_display_feature_name_using_description()
        {
            _progressNotifier.AssertWasCalled(n => n.NotifyFeatureStart("Runner tests", "desc", null));
        }

        [Fact]
        [ScenarioCategory("Category A"), ScenarioCategory("Category B"), ScenarioCategory("Category C")]
        public void Should_capture_feature_category_using_generic_category_attribute()
        {
            _subject.RunScenario(call => Step_one());
            Assert.Equal(_subject.Result.Scenarios.Single().Categories.ToArray(), new[] { "Category A", "Category B", "Category C", "Category D", "Category E" });
        }

        private void AssertStep(IEnumerable<IStepResult> steps, int number, string name, ResultStatus status, string statusDetails = null)
        {
            var result = steps.ToArray()[number - 1];
            Assert.Equal(name, result.Name);
            Assert.Equal(number, result.Number);
            Assert.Equal(status, result.Status);
            Assert.Equal(statusDetails, result.StatusDetails);
        }
    }
}
