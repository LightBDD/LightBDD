using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Formatting;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace LightBDD.Core.UnitTests.Notification
{
    [TestFixture]
    public class ParallelProgressNotifierTests
    {
        #region Capturing notifier
        class CapturingParallelProgressNotifier : ParallelProgressNotifier
        {
            private readonly AsyncLocal<int> _id = new AsyncLocal<int>();
            public ConcurrentDictionary<int, ConcurrentQueue<string>> CapturedGroups { get; } = new ConcurrentDictionary<int, ConcurrentQueue<string>>();
            public IEnumerable<string> CapturedItems => CapturedGroups.SelectMany(g => g.Value);

            public CapturingParallelProgressNotifier(ProgressManager progressManager) : base(progressManager)
            {
            }

            protected override void Notify(string message)
            {
                CapturedGroups.GetOrAdd(_id.Value, i => new ConcurrentQueue<string>()).Enqueue(message);
            }

            public void SetIdentifier(int id)
            {
                _id.Value = id;
            }
        }
        #endregion

        private CapturingParallelProgressNotifier _notifier;
        private readonly IFixture _autoFixture = new Fixture().Customize(new AutoMoqCustomization());

        public ParallelProgressNotifierTests()
        {
            _autoFixture.Register(() => new ExecutionTime(DateTimeOffset.Now, TimeSpan.FromMilliseconds(2634723)));
            _autoFixture.Register<IFeatureInfo>(() => _autoFixture.Create<TestFeatureInfo>());
            _autoFixture.Register<IFeatureResult>(() => _autoFixture.Create<TestFeatureResult>());
            _autoFixture.Register<IScenarioInfo>(() => _autoFixture.Create<TestScenarioInfo>());
            _autoFixture.Register<IScenarioResult>(() => _autoFixture.Create<TestScenarioResult>());
            _autoFixture.Register<IStepInfo>(() => _autoFixture.Create<TestStepInfo>());
            _autoFixture.Register<IStepResult>(() => _autoFixture.Create<TestStepResult>());
            _autoFixture.Register<IStepNameInfo>(() => _autoFixture.Create<TestStepNameInfo>());
            _autoFixture.Register<INameInfo>(() => _autoFixture.Create<TestNameInfo>());
        }

        [SetUp]
        public void SetUp()
        {
            _notifier = new CapturingParallelProgressNotifier(new ParallelProgressNotifier.ProgressManager());
        }

        [Test]
        public void It_should_capture_meaningful_information()
        {
            var featureInfo = _autoFixture.Create<IFeatureInfo>();
            var scenarioInfo = _autoFixture.Create<IScenarioInfo>();
            var stepInfo = _autoFixture.Create<IStepInfo>();
            var stepResult = _autoFixture.Create<IStepResult>();
            var scenarioResult = _autoFixture.Build<TestScenarioResult>().With(r => r.Status, ExecutionStatus.Passed).Create();
            var featureResult = _autoFixture.Create<IFeatureResult>();
            var comment = _autoFixture.Create<string>();

            _notifier.NotifyFeatureStart(featureInfo);
            _notifier.NotifyScenarioStart(scenarioInfo);
            _notifier.NotifyStepStart(stepInfo);
            _notifier.NotifyStepComment(stepInfo, comment);
            _notifier.NotifyStepFinished(stepResult);
            _notifier.NotifyScenarioFinished(scenarioResult);
            _notifier.NotifyFeatureFinished(featureResult);

            var headerLength = "Fi=000,Fa=000,Pe=000 #   > ".Length;
            var expected = new[]
            {
                $"Fi=000,Fa=000,Pe=000 #   > FEATURE: [{string.Join("][", featureInfo.Labels)}] {featureInfo.Name}\n{new string(' ',headerLength)}  {featureInfo.Description}",
                $"Fi=000,Fa=000,Pe=001 #  1> SCENARIO: [{string.Join("][", scenarioInfo.Labels)}] {scenarioInfo.Name}",
                $"Fi=000,Fa=000,Pe=001 #  1>   STEP {stepInfo.Number}/{stepInfo.Total}: {stepInfo.Name}...",
                $"Fi=000,Fa=000,Pe=001 #  1>   STEP {stepInfo.Number}/{stepInfo.Total}: /* {comment} */",
                $"Fi=000,Fa=000,Pe=001 #  1>   STEP {stepResult.Info.Number}/{stepResult.Info.Total}: {stepResult.Info.Name} ({stepResult.Status} after {stepResult.ExecutionTime.Duration.FormatPretty()})",
                $"Fi=001,Fa=000,Pe=000 #  1>   SCENARIO RESULT: {scenarioResult.Status} after {scenarioResult.ExecutionTime.Duration.FormatPretty()}\n{new string(' ',headerLength)}    {scenarioResult.StatusDetails}",
                $"Fi=001,Fa=000,Pe=000 #   > FEATURE FINISHED: {featureResult.Info.Name}"
            };

            Assert.That(_notifier.CapturedItems.ToArray(), Is.EqualTo(expected));
        }

        [Test]
        public void NotifyFeatureStart_should_omit_description_if_not_provided()
        {
            var featureInfo = _autoFixture.Build<TestFeatureInfo>().Without(i => i.Description).Create();
            _notifier.NotifyFeatureStart(featureInfo);

            Assert.That(_notifier.CapturedItems.Single(), Is.EqualTo($"Fi=000,Fa=000,Pe=000 #   > FEATURE: [{string.Join("][", featureInfo.Labels)}] {featureInfo.Name}"));
        }

        [Test]
        public void NotifyFeatureStart_should_omit_labels_if_not_provided()
        {
            var featureInfo = _autoFixture.Build<TestFeatureInfo>().With(i => i.Labels, Enumerable.Empty<string>()).Create();
            _notifier.NotifyFeatureStart(featureInfo);

            var header = "Fi=000,Fa=000,Pe=000 #   > ";
            var expected = $"{header}FEATURE: {featureInfo.Name}\n{new string(' ', header.Length)}  {featureInfo.Description}";
            Assert.That(_notifier.CapturedItems.Single(), Is.EqualTo(expected));
        }

        [Test]
        public void NotifyScenarioStart_should_omit_labels_if_not_provided()
        {
            var scenarioInfo = _autoFixture.Build<TestScenarioInfo>().With(i => i.Labels, Enumerable.Empty<string>()).Create();
            _notifier.NotifyScenarioStart(scenarioInfo);

            var expected = $"Fi=000,Fa=000,Pe=001 #  1> SCENARIO: {scenarioInfo.Name}";
            Assert.That(_notifier.CapturedItems.Single(), Is.EqualTo(expected));
        }

        [Test]
        public void NotifyScenarioStart_should_increase_pending_counter()
        {
            var scenarioInfo = _autoFixture.Build<TestScenarioInfo>().With(i => i.Labels, Enumerable.Empty<string>()).Create();
            var scenarioInfo2 = _autoFixture.Build<TestScenarioInfo>().With(i => i.Labels, Enumerable.Empty<string>()).Create();
            _notifier.NotifyScenarioStart(scenarioInfo);
            _notifier.NotifyScenarioStart(scenarioInfo2);

            var expected = new[]
            {
                $"Fi=000,Fa=000,Pe=001 #  1> SCENARIO: {scenarioInfo.Name}",
                $"Fi=000,Fa=000,Pe=002 #  2> SCENARIO: {scenarioInfo2.Name}"

            };
            Assert.That(_notifier.CapturedItems.ToArray(), Is.EqualTo(expected));
        }

        [Test]
        public void NotifyScenarioFinished_should_decrease_pending_counter_and_increase_finished_and_failed_counters_accordingly()
        {
            var scenarioInfo = _autoFixture.Build<TestScenarioInfo>().With(i => i.Labels, Enumerable.Empty<string>()).Create();
            var scenarioInfo2 = _autoFixture.Build<TestScenarioInfo>().With(i => i.Labels, Enumerable.Empty<string>()).Create();

            var scenarioResult = _autoFixture.Build<TestScenarioResult>().With(r => r.Info, scenarioInfo).With(r => r.Status, ExecutionStatus.Passed).Create();
            var scenarioResult2 = _autoFixture.Build<TestScenarioResult>().With(r => r.Info, scenarioInfo2).With(r => r.Status, ExecutionStatus.Failed).Create();

            _notifier.NotifyScenarioStart(scenarioInfo);
            _notifier.NotifyScenarioFinished(scenarioResult);
            _notifier.NotifyScenarioStart(scenarioInfo2);
            _notifier.NotifyScenarioFinished(scenarioResult2);

            var headerLength = "Fi=000,Fa=000,Pe=001 #  1> ".Length;

            var expected = new[]
            {
                $"Fi=000,Fa=000,Pe=001 #  1> SCENARIO: {scenarioInfo.Name}",
                $"Fi=001,Fa=000,Pe=000 #  1>   SCENARIO RESULT: {scenarioResult.Status} after {scenarioResult.ExecutionTime.Duration.FormatPretty()}\n{new string(' ',headerLength)}    {scenarioResult.StatusDetails}",
                $"Fi=001,Fa=000,Pe=001 #  2> SCENARIO: {scenarioInfo2.Name}",
                $"Fi=002,Fa=001,Pe=000 #  2>   SCENARIO RESULT: {scenarioResult2.Status} after {scenarioResult2.ExecutionTime.Duration.FormatPretty()}\n{new string(' ',headerLength)}    {scenarioResult2.StatusDetails}",

            };
            Assert.That(_notifier.CapturedItems.ToArray(), Is.EqualTo(expected));
        }

        [Test]
        public void NotifyScenarioFinished_should_omit_execution_time_if_not_provided()
        {
            var scenarioInfo = _autoFixture.Build<TestScenarioInfo>().With(i => i.Labels, Enumerable.Empty<string>()).Create();
            var scenarioResult = _autoFixture.Build<TestScenarioResult>().With(r => r.Info, scenarioInfo).With(r => r.Status, ExecutionStatus.Passed).Without(r => r.ExecutionTime).Create();

            _notifier.NotifyScenarioStart(scenarioInfo);
            _notifier.NotifyScenarioFinished(scenarioResult);

            var headerLength = "Fi=000,Fa=000,Pe=001 #  1> ".Length;

            var expected = new[]
            {
                $"Fi=000,Fa=000,Pe=001 #  1> SCENARIO: {scenarioInfo.Name}",
                $"Fi=001,Fa=000,Pe=000 #  1>   SCENARIO RESULT: {scenarioResult.Status}\n{new string(' ',headerLength)}    {scenarioResult.StatusDetails}",
            };
            Assert.That(_notifier.CapturedItems.ToArray(), Is.EqualTo(expected));
        }

        [Test]
        public void NotifyScenarioFinished_should_omit_status_details_if_not_provided()
        {
            var scenarioInfo = _autoFixture.Build<TestScenarioInfo>().With(i => i.Labels, Enumerable.Empty<string>()).Create();
            var scenarioResult = _autoFixture.Build<TestScenarioResult>().With(r => r.Info, scenarioInfo).With(r => r.Status, ExecutionStatus.Passed).Without(r => r.StatusDetails).Create();

            _notifier.NotifyScenarioStart(scenarioInfo);
            _notifier.NotifyScenarioFinished(scenarioResult);

            var expected = new[]
            {
                $"Fi=000,Fa=000,Pe=001 #  1> SCENARIO: {scenarioInfo.Name}",
                $"Fi=001,Fa=000,Pe=000 #  1>   SCENARIO RESULT: {scenarioResult.Status} after {scenarioResult.ExecutionTime.Duration.FormatPretty()}",
            };
            Assert.That(_notifier.CapturedItems.ToArray(), Is.EqualTo(expected));
        }

        [Test]
        public async Task It_should_capture_progress_properly()
        {
            var count = 1000;
            await Task.WhenAll(Enumerable.Range(0, count).Select(BeginNotification).ToArray());

            Assert.That(_notifier.CapturedGroups.Keys.Count, Is.EqualTo(count), "Not all groups has been captured");

            foreach (var group in _notifier.CapturedGroups.Values)
            {
                var identifiers = group.Select(v => Regex.Match(v, "^[^#]+#([^>]+)>").Groups[1].Value).Distinct().Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
                Assert.That(identifiers.Length, Is.EqualTo(1), "Expected one identifier in group, got: {0}", string.Join(", ", identifiers.Select(i => $"'{i}'")));
            }

            var finished = _notifier.CapturedGroups.Values.SelectMany(v => v)
                .Select(v => Regex.Match(v, "^Fi=([^,]+),").Groups[1].Value)
                .Select(int.Parse)
                .Distinct()
                .ToArray();
            Assert.That(finished, Does.Contain(count), $"Expected at least one status with Fi={count}");
        }

        private async Task BeginNotification(int i)
        {
            await Task.Yield();
            _notifier.SetIdentifier(i);
            _notifier.NotifyFeatureStart(_autoFixture.Create<IFeatureInfo>());
            await Task.Yield();
            _notifier.NotifyScenarioStart(_autoFixture.Create<IScenarioInfo>());
            await Task.Yield();
            _notifier.NotifyStepStart(_autoFixture.Create<IStepInfo>());
            await Task.Yield();
            _notifier.NotifyStepComment(_autoFixture.Create<IStepInfo>(), "comment");
            await Task.Yield();
            _notifier.NotifyStepComment(_autoFixture.Create<IStepInfo>(), "comment2");
            await Task.Yield();
            _notifier.NotifyStepFinished(_autoFixture.Create<IStepResult>());
            await Task.Yield();
            _notifier.NotifyStepStart(_autoFixture.Create<IStepInfo>());
            await Task.Yield();
            _notifier.NotifyStepComment(_autoFixture.Create<IStepInfo>(), "comment");
            await Task.Yield();
            _notifier.NotifyStepComment(_autoFixture.Create<IStepInfo>(), "comment2");
            await Task.Yield();
            _notifier.NotifyStepFinished(_autoFixture.Create<IStepResult>());
            await Task.Yield();
            _notifier.NotifyScenarioFinished(_autoFixture.Create<IScenarioResult>());
            await Task.Yield();
            _notifier.NotifyFeatureFinished(_autoFixture.Create<IFeatureResult>());
        }
        #region Mockable objects

        public class TestNameInfo : INameInfo
        {
            public string NameFormat { get; set; }
            public IEnumerable<INameParameterInfo> Parameters { get; set; }
            public override string ToString()
            {
                return NameFormat;
            }
        }

        public class TestStepNameInfo : IStepNameInfo
        {
            public string NameFormat { get; set; }
            public IEnumerable<INameParameterInfo> Parameters { get; set; }
            public string StepTypeName { get; set; }

            public override string ToString()
            {
                return NameFormat;
            }
        }

        public class TestStepResult : IStepResult
        {
            public IStepInfo Info { get; set; }
            public ExecutionStatus Status { get; set; }
            public string StatusDetails { get; set; }
            public ExecutionTime ExecutionTime { get; set; }
            public IEnumerable<string> Comments { get; set; }
        }

        public class TestStepInfo : IStepInfo
        {
            public IStepNameInfo Name { get; set; }
            public int Number { get; set; }
            public int Total { get; set; }
        }

        public class TestScenarioResult : IScenarioResult
        {
            public IScenarioInfo Info { get; set; }
            public ExecutionStatus Status { get; set; }
            public string StatusDetails { get; set; }
            public ExecutionTime ExecutionTime { get; set; }
            public IEnumerable<IStepResult> GetSteps()
            {
                return Enumerable.Empty<IStepResult>();
            }
        }

        public class TestScenarioInfo : IScenarioInfo
        {
            public INameInfo Name { get; set; }
            public IEnumerable<string> Labels { get; set; }
            public IEnumerable<string> Categories { get; set; }
        }

        public class TestFeatureResult : IFeatureResult
        {
            public IFeatureInfo Info { get; set; }

            public IEnumerable<IScenarioResult> GetScenarios()
            {
                return Enumerable.Empty<IScenarioResult>();
            }
        }

        public class TestFeatureInfo : IFeatureInfo
        {
            public INameInfo Name { get; set; }
            public IEnumerable<string> Labels { get; set; }
            public string Description { get; set; }
        }
        #endregion
    }
}