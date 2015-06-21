using LightBDD.Notification;
using LightBDD.Results;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.UnitTests.Notification
{
    [TestFixture]
    public class SimplifiedConsoleProgressNotifierTests
    {
        private ConsoleInterceptor _console;
        private SimplifiedConsoleProgressNotifier _subject;

        [SetUp]
        public void SetUp()
        {
            _console = new ConsoleInterceptor();
            _subject = SimplifiedConsoleProgressNotifier.GetInstance();
            _subject.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            _console.Dispose();
        }

        [Test]
        public void NotifyFeatureStart_should_print_nothing()
        {
            _subject.NotifyFeatureStart("a", "b", "c");
            Assert.That(_console.GetCapturedText(), Is.EqualTo(string.Empty));
        }

        [Test]
        public void NotifyStepStart_should_print_nothing()
        {
            _subject.NotifyStepStart("a", 1, 1);
            Assert.That(_console.GetCapturedText(), Is.EqualTo(string.Empty));
        }

        [Test]
        public void NotifyStepFinished_should_print_nothing()
        {
            _subject.NotifyStepFinished(MockRepository.GenerateMock<IStepResult>(), 1);
            Assert.That(_console.GetCapturedText(), Is.EqualTo(string.Empty));
        }

        [Test]
        public void NotifyStepComment_should_print_nothing()
        {
            _subject.NotifyStepComment(1, 1, "a");
            Assert.That(_console.GetCapturedText(), Is.EqualTo(string.Empty));
        }

        [Test]
        public void NotifyScenarioStart_should_print_notification_and_update_stats()
        {
            _subject.NotifyScenarioStart("name", "l");
            Assert.That(_console.GetCapturedText(), Is.EqualTo("Finished=0, Failed=0, Pending=1: starting: name\r\n"));
            _console.Reset();

            _subject.NotifyScenarioStart("name2", "l");
            Assert.That(_console.GetCapturedText(), Is.EqualTo("Finished=0, Failed=0, Pending=2: starting: name2\r\n"));
        }

        [Test]
        [TestCase(ResultStatus.Failed, 1)]
        [TestCase(ResultStatus.Bypassed, 0)]
        [TestCase(ResultStatus.Ignored, 0)]
        [TestCase(ResultStatus.Passed, 0)]
        public void NotifyScenarioFinished_should_print_notification_and_update_stats(ResultStatus status, int expectedFailures)
        {
            _subject.NotifyScenarioStart("name", "l");
            _console.Reset();
            _subject.NotifyScenarioFinished(CreateScenarioResult(status, "name"));
            Assert.That(_console.GetCapturedText(), Is.EqualTo(string.Format("Finished=1, Failed={0}, Pending=0: {1}: name\r\n", expectedFailures, status.ToString().ToUpperInvariant())));
        }

        private static IScenarioResult CreateScenarioResult(ResultStatus status, string name)
        {
            var result = MockRepository.GenerateMock<IScenarioResult>();
            result.Stub(r => r.Name).Return(name);
            result.Stub(r => r.Status).Return(status);
            return result;
        }

        [Test]
        public void NotifyScenarioFinished_should_update_stats_properly_for_multiple_calls()
        {
            _subject.NotifyScenarioStart("name", "l");
            _subject.NotifyScenarioStart("name2", "l");
            _subject.NotifyScenarioStart("name3", "l");
            _console.Reset();

            _subject.NotifyScenarioFinished(CreateScenarioResult(ResultStatus.Failed, "name"));
            Assert.That(_console.GetCapturedText(), Is.EqualTo("Finished=1, Failed=1, Pending=2: FAILED: name\r\n"));
            _console.Reset();

            _subject.NotifyScenarioFinished(CreateScenarioResult(ResultStatus.Ignored, "name2"));
            Assert.That(_console.GetCapturedText(), Is.EqualTo("Finished=2, Failed=1, Pending=1: IGNORED: name2\r\n"));
            _console.Reset();

            _subject.NotifyScenarioFinished(CreateScenarioResult(ResultStatus.Failed, "name3"));
            Assert.That(_console.GetCapturedText(), Is.EqualTo("Finished=3, Failed=2, Pending=0: FAILED: name3\r\n"));
        }
    }
}