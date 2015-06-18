using System;
using LightBDD.Notification;
using LightBDD.Results;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.UnitTests
{
    [TestFixture]
    [Label("Ticket-1"), FeatureDescription("Runner tests description")]
    public class BDD_runner_progress_notification_tests : SomeSteps
    {
        private AbstractBDDRunner _subject;
        private IProgressNotifier _progressNotifier;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _progressNotifier = MockRepository.GenerateMock<IProgressNotifier>();
            _subject = new TestableBDDRunner(GetType(), _progressNotifier);
        }

        #endregion

        [Test]
        public void Should_display_feature_name()
        {
            _progressNotifier.AssertWasCalled(n => n.NotifyFeatureStart("BDD runner progress notification tests", "Runner tests description", "Ticket-1"));
        }

        [Test]
        public void Should_display_scenario_failure()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _subject.RunScenario(Step_throwing_exception));
            _progressNotifier.AssertWasCalled(n => n.NotifyScenarioFinished(Arg<IScenarioResult>.Matches(r => r.Status == ResultStatus.Failed && r.StatusDetails == "Step 1: " + ex.Message)));
        }

        [Test]
        public void Should_display_scenario_ignored()
        {
            var ex = Assert.Throws<IgnoreException>(() => _subject.RunScenario(Step_with_ignore_assertion));
            _progressNotifier.AssertWasCalled(n => n.NotifyScenarioFinished(Arg<IScenarioResult>.Matches(r => r.Status == ResultStatus.Ignored && r.StatusDetails == "Step 1: " + ex.Message)));
        }

        [Test]
        public void Should_display_scenario_success()
        {
            _subject.RunScenario(Step_one);
            _progressNotifier.AssertWasCalled(n => n.NotifyScenarioFinished(Arg<IScenarioResult>.Matches(r => r.Status == ResultStatus.Passed)));
        }

        [Test]
        [Label("Ticket-2")]
        public void Should_display_scenario_name()
        {
            _subject.RunScenario(Step_one);
            _progressNotifier.AssertWasCalled(n => n.NotifyScenarioStart("Should display scenario name", "Ticket-2"));
        }

        [Test]
        public void Should_display_steps()
        {
            Assert.Throws<InvalidOperationException>(() => _subject.RunScenario(Step_one, Step_throwing_exception));
            _progressNotifier.AssertWasCalled(n => n.NotifyStepStart("Step one", 1, 2));
            _progressNotifier.AssertWasCalled(n => n.NotifyStepFinished(Arg<IStepResult>.Matches(r => r.Number == 1 && r.Status == ResultStatus.Passed), Arg<int>.Is.Equal(2)));
            _progressNotifier.AssertWasCalled(n => n.NotifyStepStart("Step throwing exception", 2, 2));
            _progressNotifier.AssertWasCalled(n => n.NotifyStepFinished(Arg<IStepResult>.Matches(r => r.Number == 2 && r.Status == ResultStatus.Failed), Arg<int>.Is.Equal(2)));
        }

        [Test]
        public void Should_display_parameterized_steps()
        {
            Assert.Throws<InvalidOperationException>(() => _subject.RunScenario(
                call => Step_one(),
                call => Step_throwing_exception_MESSAGE("abc")));

            _progressNotifier.AssertWasCalled(n => n.NotifyStepStart("CALL Step one", 1, 2));
            _progressNotifier.AssertWasCalled(n => n.NotifyStepFinished(Arg<IStepResult>.Matches(r => r.Number == 1 && r.Status == ResultStatus.Passed), Arg<int>.Is.Equal(2)));
            _progressNotifier.AssertWasCalled(n => n.NotifyStepStart("CALL Step throwing exception \"abc\"", 2, 2));
            _progressNotifier.AssertWasCalled(n => n.NotifyStepFinished(Arg<IStepResult>.Matches(r => r.Number == 2 && r.Status == ResultStatus.Failed), Arg<int>.Is.Equal(2)));
        }

        [Test]
        public void Should_display_comment()
        {
            _subject.RunScenario(call => Step_with_comments());
            _progressNotifier.AssertWasCalled(n => n.NotifyStepStart("CALL Step with comments", 1, 1));
            _progressNotifier.AssertWasCalled(n => n.NotifyStepComment(1, 1, "comment one"));
            _progressNotifier.AssertWasCalled(n => n.NotifyStepComment(1, 1, "comment 2"));
            _progressNotifier.AssertWasCalled(n => n.NotifyStepFinished(Arg<IStepResult>.Matches(r => r.Number == 1 && r.Status == ResultStatus.Passed), Arg<int>.Is.Equal(1)));
        }

        [Test]
        [TestCase(null)]
        [TestCase("\r\n \t")]
        public void Should_not_display_empty_comments(string comment)
        {
            _subject.RunScenario(call => Step_with_comment(comment));
            _progressNotifier.AssertWasCalled(n => n.NotifyStepStart(Arg<string>.Is.Anything, Arg<int>.Is.Anything, Arg<int>.Is.Anything));
            _progressNotifier.AssertWasNotCalled(n => n.NotifyStepComment(Arg<int>.Is.Anything, Arg<int>.Is.Anything, Arg<string>.Is.Anything));
            _progressNotifier.AssertWasCalled(n => n.NotifyStepFinished(Arg<IStepResult>.Matches(r => r.Number == 1 && r.Status == ResultStatus.Passed), Arg<int>.Is.Equal(1)));
        }
    }
}