using LightBDD.Core.Notification;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.Core.UnitTests.Notification
{
    [TestFixture]
    public class DelegatingScenarioProgressNotifier_tests
    {
        private DelegatingScenarioProgressNotifier _subject;
        private IScenarioProgressNotifier[] _notifiers;

        [SetUp]
        public void SetUp()
        {
            _notifiers = new[] { MockRepository.GenerateMock<IScenarioProgressNotifier>(), MockRepository.GenerateMock<IScenarioProgressNotifier>() };
            _subject = new DelegatingScenarioProgressNotifier(_notifiers);
        }
        
        [Test]
        public void It_should_delegate_NotifyStepStart()
        {
            var stepInfo = new Mocks.TestStepInfo();
            _subject.NotifyStepStart(stepInfo);
            foreach (var notifier in _notifiers)
                notifier.AssertWasCalled(n => n.NotifyStepStart(stepInfo));
        }

        [Test]
        public void It_should_delegate_NotifyStepFinished()
        {
            var step = new Mocks.TestStepResult();
            _subject.NotifyStepFinished(step);
            foreach (var notifier in _notifiers)
                notifier.AssertWasCalled(n => n.NotifyStepFinished(step));
        }

        [Test]
        public void It_should_delegate_NotifyStepComment()
        {
            var stepInfo = new Mocks.TestStepInfo();
            var comment = "comment";
            _subject.NotifyStepComment(stepInfo, comment);
            foreach (var notifier in _notifiers)
                notifier.AssertWasCalled(n => n.NotifyStepComment(stepInfo, comment));
        }

        [Test]
        public void It_should_delegate_NotifyScenarioStart()
        {
            var scenarioInfo = new Mocks.TestScenarioInfo();
            _subject.NotifyScenarioStart(scenarioInfo);
            foreach (var notifier in _notifiers)
                notifier.AssertWasCalled(n => n.NotifyScenarioStart(scenarioInfo));
        }

        [Test]
        public void It_should_delegate_NotifyScenarioFinished_should_print_notification_and_update_stats()
        {
            var scenario = new Mocks.TestScenarioResult();
            _subject.NotifyScenarioFinished(scenario);
            foreach (var notifier in _notifiers)
                notifier.AssertWasCalled(n => n.NotifyScenarioFinished(scenario));
        }
    }
}