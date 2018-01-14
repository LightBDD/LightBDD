using System;
using LightBDD.Core.Notification;
using LightBDD.Framework.Notification;
using LightBDD.UnitTests.Helpers;
using Moq;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Notification
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

        [Test]
        public void Constructor_should_not_accept_null_collection_nor_null_items()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new DelegatingScenarioProgressNotifier(null));
            Assert.That(ex.ParamName, Is.EqualTo("notifiers"));

            ex = Assert.Throws<ArgumentNullException>(() => new DelegatingScenarioProgressNotifier(Mock.Of<IScenarioProgressNotifier>(), null));
            Assert.That(ex.ParamName, Is.EqualTo("notifiers[1]"));
        }

        [Test]
        public void Compose_should_flatten_notifiers_and_return_NoProgressNotifier_if_no_specific_notifiers_are_present()
        {
            var result = DelegatingScenarioProgressNotifier.Compose(new IScenarioProgressNotifier[]
            {
                new DelegatingScenarioProgressNotifier(
                    NoProgressNotifier.Default,
                    NoProgressNotifier.Default,
                    new DelegatingScenarioProgressNotifier(),
                    new DelegatingScenarioProgressNotifier(NoProgressNotifier.Default)),
                NoProgressNotifier.Default,
                new DelegatingScenarioProgressNotifier()
            });

            Assert.That(result, Is.TypeOf<NoProgressNotifier>());
        }

        [Test]
        public void Compose_should_flatten_notifiers_and_return_specific_one_if_only_one_is_present()
        {
            var specific = Mock.Of<IScenarioProgressNotifier>();
            var result = DelegatingScenarioProgressNotifier.Compose(new IScenarioProgressNotifier[]
            {
                new DelegatingScenarioProgressNotifier(
                    NoProgressNotifier.Default,
                    NoProgressNotifier.Default,
                    new DelegatingScenarioProgressNotifier(),
                    new DelegatingScenarioProgressNotifier(NoProgressNotifier.Default, specific)),
                NoProgressNotifier.Default,
                new DelegatingScenarioProgressNotifier()
            });

            Assert.That(result, Is.SameAs(specific));
        }

        [Test]
        public void Compose_should_flatten_notifiers()
        {
            var specific1 = Mock.Of<IScenarioProgressNotifier>();
            var specific2 = Mock.Of<IScenarioProgressNotifier>();
            var result = DelegatingScenarioProgressNotifier.Compose(new[]
            {
                new DelegatingScenarioProgressNotifier(
                    NoProgressNotifier.Default,
                    NoProgressNotifier.Default,
                    new DelegatingScenarioProgressNotifier(),
                    new DelegatingScenarioProgressNotifier(NoProgressNotifier.Default, specific1)),
                NoProgressNotifier.Default,
                new DelegatingScenarioProgressNotifier(),
                specific2
            });

            Assert.That(result, Is.TypeOf<DelegatingScenarioProgressNotifier>());
            Assert.That(
                ((DelegatingScenarioProgressNotifier)result).Notifiers,
                Is.EqualTo(new[] { specific1, specific2 }));
        }
    }
}