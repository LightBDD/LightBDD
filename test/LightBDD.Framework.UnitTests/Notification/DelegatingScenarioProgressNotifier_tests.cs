using LightBDD.Core.Notification;
using LightBDD.Framework.Notification;
using LightBDD.UnitTests.Helpers;
using Moq;
using NUnit.Framework;

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
            _notifiers = new[] { Mock.Of<IScenarioProgressNotifier>(), Mock.Of<IScenarioProgressNotifier>() };
            _subject = new DelegatingScenarioProgressNotifier(_notifiers);
        }
        
        [Test]
        public void It_should_delegate_NotifyStepStart()
        {
            var stepInfo = new TestResults.TestStepInfo();
            _subject.NotifyStepStart(stepInfo);
            foreach (var notifier in _notifiers)
                Mock.Get(notifier).Verify(n => n.NotifyStepStart(stepInfo));
        }

        [Test]
        public void It_should_delegate_NotifyStepFinished()
        {
            var step = new TestResults.TestStepResult();
            _subject.NotifyStepFinished(step);
            foreach (var notifier in _notifiers)
                Mock.Get(notifier).Verify(n => n.NotifyStepFinished(step));
        }

        [Test]
        public void It_should_delegate_NotifyStepComment()
        {
            var stepInfo = new TestResults.TestStepInfo();
            var comment = "comment";
            _subject.NotifyStepComment(stepInfo, comment);
            foreach (var notifier in _notifiers)
                Mock.Get(notifier).Verify(n => n.NotifyStepComment(stepInfo, comment));
        }

        [Test]
        public void It_should_delegate_NotifyScenarioStart()
        {
            var scenarioInfo = new TestResults.TestScenarioInfo();
            _subject.NotifyScenarioStart(scenarioInfo);
            foreach (var notifier in _notifiers)
                Mock.Get(notifier).Verify(n => n.NotifyScenarioStart(scenarioInfo));
        }

        [Test]
        public void It_should_delegate_NotifyScenarioFinished_should_print_notification_and_update_stats()
        {
            var scenario = new TestResults.TestScenarioResult();
            _subject.NotifyScenarioFinished(scenario);
            foreach (var notifier in _notifiers)
                Mock.Get(notifier).Verify(n => n.NotifyScenarioFinished(scenario));
        }
    }
}