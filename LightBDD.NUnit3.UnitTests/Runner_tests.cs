using System.Collections.Generic;
using System.Linq;
using LightBDD.Notification;
using LightBDD.Results;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.NUnit3.UnitTests
{
    [TestFixture]
    [Description("desc")]
    [Category("Category D"), ScenarioCategory("Category E")]
    public class Runner_tests 
    {
        private BDDRunner _subject;
        private IProgressNotifier _progressNotifier;

        private void AssertStep(IEnumerable<IStepResult> steps, int number, string name, ResultStatus status, string statusDetails = null)
        {
            var result = steps.ToArray()[number - 1];
            Assert.That(result.Name, Is.EqualTo(name));
            Assert.That(result.Number, Is.EqualTo(number));
            Assert.That(result.Status, Is.EqualTo(status));
            Assert.That(result.StatusDetails, Is.EqualTo(statusDetails));
        }

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _progressNotifier = MockRepository.GenerateMock<IProgressNotifier>();
            _subject = new BDDRunner(GetType(), _progressNotifier);
        }

        #endregion

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
            Assert.That(result.Name, Is.EqualTo("Should collect scenario result for inconclusive scenario steps"));
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Ignored));
            Assert.That(result.Steps.Count(), Is.EqualTo(3));
            AssertStep(result.Steps, 1, "Step one", ResultStatus.Passed);
            AssertStep(result.Steps, 2, "Step with inconclusive assertion", ResultStatus.Ignored, expectedStatusDetails);
            AssertStep(result.Steps, 3, "Step two", ResultStatus.NotRun);
            Assert.That(result.StatusDetails, Is.EqualTo("Step 2: " + expectedStatusDetails));
        }

        [Test]
        public void Should_display_feature_name_using_description()
        {
            _progressNotifier.AssertWasCalled(n => n.NotifyFeatureStart("Runner tests", "desc", null));
        }

        [Test]
        [Category("Category A"), Category("Category B"), ScenarioCategory("Category C")]
        public void Should_capture_feature_category_using_generic_category_attribute()
        {
            _subject.RunScenario(call => Step_one());
            Assert.That(_subject.Result.Scenarios.Single().Categories.ToArray(), Is.EqualTo(new[] { "Category A", "Category B", "Category C", "Category D", "Category E" }));
        }

        [Test]
        public void Should_display_scenario_inconclusive()
        {
            var ex = Assert.Throws<InconclusiveException>(() => _subject.RunScenario(Step_with_inconclusive_assertion));
            _progressNotifier.AssertWasCalled(n => n.NotifyScenarioFinished(Arg<IScenarioResult>.Matches(r => r.Status == ResultStatus.Ignored && r.StatusDetails == "Step 1: " + ex.Message)));
        }

        public void Step_one() { }
        public void Step_two() { }
        public void Step_with_inconclusive_assertion() { Assert.Inconclusive("some reason"); }
    }
}
