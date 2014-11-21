using System.Collections.Generic;
using System.Linq;
using Gallio.Framework;
using LightBDD.Notification;
using LightBDD.Results;
using MbUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.MbUnit.UnitTests
{
    [TestFixture]
    [Description("desc"), Category("Category A"), Category("Category B"), FeatureCategory("Category C")]
    public class Runner_tests
    {
        private IProgressNotifier _progressNotifier;
        private BDDRunner _subject;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _progressNotifier = MockRepository.GenerateMock<IProgressNotifier>();
            _subject = new BDDRunner(GetType(), _progressNotifier);
        }

        #endregion

        public void Step_one() { }
        public void Step_two() { }
        public void Step_with_inconclusive_assertion() { Assert.Inconclusive("some reason"); }

        [Test]
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
            Assert.AreEqual("Should collect scenario result for inconclusive scenario steps", result.Name);
            Assert.AreEqual(ResultStatus.Ignored, result.Status);

            Assert.AreEqual(3, result.Steps.Count());
            AssertStep(result.Steps, 1, "Step one", ResultStatus.Passed);
            AssertStep(result.Steps, 2, "Step with inconclusive assertion", ResultStatus.Ignored, expectedStatusDetails);
            AssertStep(result.Steps, 3, "Step two", ResultStatus.NotRun);

            Assert.AreEqual("Step 2: " + expectedStatusDetails, result.StatusDetails);
        }

        [Test]
        public void Should_display_feature_name_using_description()
        {
            _progressNotifier.AssertWasCalled(n => n.NotifyFeatureStart("Runner tests", "desc", null));
        }

        [Test]
        public void Should_capture_feature_category_using_generic_category_attribute()
        {
            Assert.AreElementsEqual(new[] { "Category A", "Category B", "Category C" }, _subject.Result.Categories.ToArray());
        }

        [Test]
        public void Should_display_scenario_inconclusive()
        {
            var ex = Assert.Throws<TestInconclusiveException>(() => _subject.RunScenario(Step_with_inconclusive_assertion));
            _progressNotifier.AssertWasCalled(n => n.NotifyScenarioFinished(Arg<IScenarioResult>.Matches(r => r.Status == ResultStatus.Ignored && r.StatusDetails == "Step 1: " + ex.Message)));
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
