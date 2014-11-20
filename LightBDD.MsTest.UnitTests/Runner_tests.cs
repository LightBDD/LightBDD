using System.Collections.Generic;
using System.Linq;
using LightBDD.Notification;
using LightBDD.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace LightBDD.MsTest.UnitTests
{
    [TestClass]
    [FeatureDescription("desc")]
    [FeatureCategory("Category B")]
    [FeatureCategory("Category A")]
    public class Runner_tests
    {
        private IProgressNotifier _progressNotifier;
        private BDDRunner _subject;

        #region Setup/Teardown

        [TestInitialize]
        public void SetUp()
        {
            _progressNotifier = MockRepository.GenerateMock<IProgressNotifier>();
            _subject = new BDDRunner(GetType(), _progressNotifier);
        }

        #endregion

        public void Step_one() { }
        public void Step_two() { }
        public void Step_with_inconclusive_assertion() { Assert.Inconclusive("some reason"); }

        [TestMethod]
        public void Should_collect_scenario_result_for_inconclusive_scenario_steps()
        {
            try
            {
                _subject.RunScenario(Step_one, Step_with_inconclusive_assertion, Step_two);
            }
            catch
            {
            }
            const string expectedStatusDetails = "Assert.Inconclusive failed. some reason";

            var result = _subject.Result.Scenarios.Single();
            Assert.AreEqual("Should collect scenario result for inconclusive scenario steps", result.Name);
            Assert.AreEqual(ResultStatus.Ignored, result.Status);

            Assert.AreEqual(3, result.Steps.Count());
            AssertStep(result.Steps, 1, "Step one", ResultStatus.Passed);
            AssertStep(result.Steps, 2, "Step with inconclusive assertion", ResultStatus.Ignored, expectedStatusDetails);
            AssertStep(result.Steps, 3, "Step two", ResultStatus.NotRun);

            Assert.AreEqual(expectedStatusDetails, result.StatusDetails);
        }

        [TestMethod]
        public void Should_display_feature_name_using_generic_description()
        {
            _progressNotifier.AssertWasCalled(n => n.NotifyFeatureStart("Runner tests", "desc", null));
        }

        [TestMethod]
        public void Should_capture_feature_category_using_generic_category_attribute()
        {
            CollectionAssert.AreEqual(new[] { "Category A", "Category B" }, _subject.Result.Categories.ToArray());
        }

        [TestMethod]
        public void Should_display_scenario_inconclusive()
        {
            try
            {
                _subject.RunScenario(Step_with_inconclusive_assertion);
                Assert.Fail("Inconclusive exception was expected");
            }
            catch (AssertInconclusiveException ex)
            {
                _progressNotifier.AssertWasCalled(n => n.NotifyScenarioFinished(Arg<IScenarioResult>.Matches(r => r.Status == ResultStatus.Ignored && r.StatusDetails == ex.Message)));
            }
        }

        private void AssertStep(IEnumerable<IStepResult> steps, int number, string name, ResultStatus status, string statusDetails = null)
        {
            var result = steps.ToArray()[number - 1];
            Assert.AreEqual(name, result.Name);
            Assert.AreEqual(number, result.Number);
            Assert.AreEqual(status, result.Status);
            Assert.AreEqual(statusDetails, result.StatusDetails);
        }
    }
}
