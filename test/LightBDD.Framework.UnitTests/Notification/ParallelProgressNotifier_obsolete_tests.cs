using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Formatting;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Parameters;
using LightBDD.Core.Results.Parameters.Tabular;
using LightBDD.Framework.Notification;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Notification
{
    [TestFixture]
    [Obsolete]
    public class ParallelProgressNotifier_obsolete_tests
    {
        private ConcurrentDictionary<int, ConcurrentQueue<string>> _capturedGroups;
        public IEnumerable<string> CapturedItems => _capturedGroups.SelectMany(g => g.Value);
        private readonly AsyncLocal<int> _currentId = new();
        private ParallelProgressNotifierProvider _notifierProvider;

        private class TestableParallelProgressNotifierProvider : ParallelProgressNotifierProvider { }

        private void Notify(string message)
        {
            _capturedGroups.GetOrAdd(_currentId.Value, i => new ConcurrentQueue<string>()).Enqueue(message);
        }

        public IFeatureProgressNotifier GetFeatureNotifier()
        {
            return _notifierProvider.CreateFeatureProgressNotifier(Notify);
        }

        public IScenarioProgressNotifier GetScenarioNotifier()
        {
            return _notifierProvider.CreateScenarioProgressNotifier(Notify);
        }

        [SetUp]
        public void SetUp()
        {
            _capturedGroups = new ConcurrentDictionary<int, ConcurrentQueue<string>>();
            _notifierProvider = new TestableParallelProgressNotifierProvider();
        }

        [Test]
        public void It_should_capture_meaningful_information()
        {
            var featureInfo = Fake.Object<TestResults.TestFeatureInfo>();
            var scenarioInfo = Fake.Object<TestResults.TestScenarioInfo>();
            var stepInfo = Fake.Object<TestResults.TestStepInfo>();
            var stepResult = Fake.Object<TestResults.TestStepResult>();
            stepResult.Parameters = new IParameterResult[]
            {
                new TestResults.TestParameterResult("table",
                    TestResults.CreateTabularParameterDetails(ParameterVerificationStatus.Failure)
                        .WithKeyColumns("Key")
                        .WithValueColumns("Value1", "Value2")
                        .AddRow(TableRowType.Matching,
                            ParameterVerificationStatus.Success,
                            TestResults.CreateValueResult("1"),
                            TestResults.CreateValueResult("abc"),
                            TestResults.CreateValueResult("some value"))
                        .AddRow(TableRowType.Matching,
                            ParameterVerificationStatus.Failure,
                            TestResults.CreateValueResult("2"),
                            TestResults.CreateValueResult("def"),
                            TestResults.CreateValueResult("value", "val", ParameterVerificationStatus.Failure))
                        .AddRow(TableRowType.Missing,
                            ParameterVerificationStatus.Failure,
                            TestResults.CreateValueResult("3"),
                            TestResults.CreateValueResult("XXX", "<null>", ParameterVerificationStatus.NotProvided),
                            TestResults.CreateValueResult("YYY", "<null>", ParameterVerificationStatus.NotProvided))
                        .AddRow(TableRowType.Surplus,
                            ParameterVerificationStatus.Failure,
                            TestResults.CreateValueResult("4"),
                            TestResults.CreateValueResult("<null>", "XXX",
                                ParameterVerificationStatus.Failure),
                            TestResults.CreateValueResult("<null>", "YYY",
                                ParameterVerificationStatus.Failure))
                )
            };

            var scenarioResult = Fake.Object<TestResults.TestScenarioResult>();
            scenarioResult.Status = ExecutionStatus.Passed;

            var featureResult = Fake.Object<TestResults.TestFeatureResult>();
            var comment = Fake.String();

            var featureNotifier = GetFeatureNotifier();
            var scenarioNotifier = GetScenarioNotifier();

            featureNotifier.NotifyFeatureStart(featureInfo);
            scenarioNotifier.NotifyScenarioStart(scenarioInfo);
            scenarioNotifier.NotifyStepStart(stepInfo);
            scenarioNotifier.NotifyStepComment(stepInfo, comment);
            scenarioNotifier.NotifyStepFinished(stepResult);
            scenarioNotifier.NotifyScenarioFinished(scenarioResult);
            featureNotifier.NotifyFeatureFinished(featureResult);

            var headerLength = "Fi=000,Fa=000,Pe=000 #   > ".Length;
            var padding = new string(' ', headerLength);

            var expectedTable = $@"{padding}    table:
{padding}    +-+---+----------+----------+
{padding}    |#|Key|Value1    |Value2    |
{padding}    +-+---+----------+----------+
{padding}    |=|1  |abc       |some value|
{padding}    |!|2  |def       |val/value |
{padding}    |-|3  |<null>/XXX|<null>/YYY|
{padding}    |+|4  |XXX/<null>|YYY/<null>|
{padding}    +-+---+----------+----------+"
                .Replace("\r", "")
                .Replace("\n", Environment.NewLine);

            var expected = new[]
            {
                $"Fi=000,Fa=000,Pe=000 #   > FEATURE: [{string.Join("][", featureInfo.Labels)}] {featureInfo.Name}{Environment.NewLine}{padding}  {featureInfo.Description}",
                $"Fi=000,Fa=000,Pe=001 #  1> SCENARIO: [{string.Join("][", scenarioInfo.Labels)}] {scenarioInfo.Name}{Environment.NewLine}{padding}  {scenarioInfo.Description}",
                $"Fi=000,Fa=000,Pe=001 #  1>   STEP {stepInfo.GroupPrefix}{stepInfo.Number}/{stepInfo.GroupPrefix}{stepInfo.Total}: {stepInfo.Name}...",
                $"Fi=000,Fa=000,Pe=001 #  1>   STEP {stepInfo.GroupPrefix}{stepInfo.Number}/{stepInfo.GroupPrefix}{stepInfo.Total}: /* {comment} */",
                $"Fi=000,Fa=000,Pe=001 #  1>   STEP {stepResult.Info.GroupPrefix}{stepResult.Info.Number}/{stepResult.Info.GroupPrefix}{stepResult.Info.Total}: {stepResult.Info.Name} ({stepResult.Status} after {stepResult.ExecutionTime.Duration.FormatPretty()}){Environment.NewLine}{expectedTable}",
                $"Fi=001,Fa=000,Pe=000 #  1>   SCENARIO RESULT: {scenarioResult.Status} after {scenarioResult.ExecutionTime.Duration.FormatPretty()}{Environment.NewLine}{padding}    {scenarioResult.StatusDetails}",
                $"Fi=001,Fa=000,Pe=000 #   > FEATURE FINISHED: {featureResult.Info.Name}"
            };

            Assert.That(CapturedItems.ToArray(), Is.EqualTo(expected));
        }

        [Test]
        public void NotifyFeatureStart_should_omit_description_if_not_provided()
        {
            var featureInfo = Fake.Object<TestResults.TestFeatureInfo>();
            featureInfo.Description = null;
            GetFeatureNotifier().NotifyFeatureStart(featureInfo);

            Assert.That(CapturedItems.Single(), Is.EqualTo($"Fi=000,Fa=000,Pe=000 #   > FEATURE: [{string.Join("][", featureInfo.Labels)}] {featureInfo.Name}"));
        }

        [Test]
        public void NotifyFeatureStart_should_omit_labels_if_not_provided()
        {
            var featureInfo = Fake.Object<TestResults.TestFeatureInfo>();
            featureInfo.Labels = Array.Empty<string>();
            GetFeatureNotifier().NotifyFeatureStart(featureInfo);

            var header = "Fi=000,Fa=000,Pe=000 #   > ";
            var expected = $"{header}FEATURE: {featureInfo.Name}{Environment.NewLine}{new string(' ', header.Length)}  {featureInfo.Description}";
            Assert.That(CapturedItems.Single(), Is.EqualTo(expected));
        }

        [Test]
        public void NotifyScenarioStart_should_omit_labels_if_not_provided()
        {
            var scenarioInfo = Fake.Object<TestResults.TestScenarioInfo>();
            scenarioInfo.Labels = Array.Empty<string>();
            scenarioInfo.Description = null;
            GetScenarioNotifier().NotifyScenarioStart(scenarioInfo);

            var expected = $"Fi=000,Fa=000,Pe=001 #  1> SCENARIO: {scenarioInfo.Name}";
            Assert.That(CapturedItems.Single(), Is.EqualTo(expected));
        }

        [Test]
        public void NotifyScenarioStart_should_increase_pending_counter()
        {
            var scenarioInfo = Fake.Object<TestResults.TestScenarioInfo>();
            var scenarioInfo2 = Fake.Object<TestResults.TestScenarioInfo>();
            scenarioInfo.Description = null;
            scenarioInfo.Labels = Array.Empty<string>();
            scenarioInfo2.Description = null;
            scenarioInfo2.Labels = Array.Empty<string>();
            var scenarioNotifier = GetScenarioNotifier();

            scenarioNotifier.NotifyScenarioStart(scenarioInfo);
            scenarioNotifier.NotifyScenarioStart(scenarioInfo2);

            var expected = new[]
            {
                $"Fi=000,Fa=000,Pe=001 #  1> SCENARIO: {scenarioInfo.Name}",
                $"Fi=000,Fa=000,Pe=002 #  2> SCENARIO: {scenarioInfo2.Name}"

            };
            Assert.That(CapturedItems.ToArray(), Is.EqualTo(expected));
        }

        [Test]
        public void NotifyScenarioFinished_should_decrease_pending_counter_and_increase_finished_and_failed_counters_accordingly()
        {
            var scenarioInfo = Fake.Object<TestResults.TestScenarioInfo>();
            var scenarioInfo2 = Fake.Object<TestResults.TestScenarioInfo>();
            scenarioInfo.Labels = Array.Empty<string>();
            scenarioInfo2.Labels = Array.Empty<string>();

            var scenarioResult = Fake.Object<TestResults.TestScenarioResult>();
            scenarioResult.Info = scenarioInfo;
            scenarioResult.Status = ExecutionStatus.Passed;

            var scenarioResult2 = Fake.Object<TestResults.TestScenarioResult>();
            scenarioResult2.Info = scenarioInfo2;
            scenarioResult2.Status = ExecutionStatus.Failed;

            var scenarioNotifier = GetScenarioNotifier();
            scenarioNotifier.NotifyScenarioStart(scenarioInfo);
            scenarioNotifier.NotifyScenarioFinished(scenarioResult);
            scenarioNotifier.NotifyScenarioStart(scenarioInfo2);
            scenarioNotifier.NotifyScenarioFinished(scenarioResult2);

            var headerLength = "Fi=000,Fa=000,Pe=001 #  1> ".Length;
            var padding = new string(' ', headerLength);

            var expected = new[]
            {
                $"Fi=000,Fa=000,Pe=001 #  1> SCENARIO: {scenarioInfo.Name}{Environment.NewLine}{padding}  {scenarioInfo.Description}",
                $"Fi=001,Fa=000,Pe=000 #  1>   SCENARIO RESULT: {scenarioResult.Status} after {scenarioResult.ExecutionTime.Duration.FormatPretty()}{Environment.NewLine}{new string(' ',headerLength)}    {scenarioResult.StatusDetails}",
                $"Fi=001,Fa=000,Pe=001 #  2> SCENARIO: {scenarioInfo2.Name}{Environment.NewLine}{padding}  {scenarioInfo2.Description}",
                $"Fi=002,Fa=001,Pe=000 #  2>   SCENARIO RESULT: {scenarioResult2.Status} after {scenarioResult2.ExecutionTime.Duration.FormatPretty()}{Environment.NewLine}{new string(' ',headerLength)}    {scenarioResult2.StatusDetails}"

            };
            Assert.That(CapturedItems.ToArray(), Is.EqualTo(expected));
        }

        [Test]
        public void NotifyScenarioFinished_should_omit_execution_time_if_not_provided()
        {
            var scenarioInfo = Fake.Object<TestResults.TestScenarioInfo>();
            scenarioInfo.Labels = Array.Empty<string>();
            scenarioInfo.Description = null;
            var scenarioResult = Fake.Object<TestResults.TestScenarioResult>();
            scenarioResult.Info = scenarioInfo;
            scenarioResult.Status = ExecutionStatus.Passed;
            scenarioResult.ExecutionTime = null;

            var scenarioNotifier = GetScenarioNotifier();
            scenarioNotifier.NotifyScenarioStart(scenarioInfo);
            scenarioNotifier.NotifyScenarioFinished(scenarioResult);

            var headerLength = "Fi=000,Fa=000,Pe=001 #  1> ".Length;
            var padding = new string(' ', headerLength);

            var expected = new[]
            {
                $"Fi=000,Fa=000,Pe=001 #  1> SCENARIO: {scenarioInfo.Name}",
                $"Fi=001,Fa=000,Pe=000 #  1>   SCENARIO RESULT: {scenarioResult.Status}{Environment.NewLine}{padding}    {scenarioResult.StatusDetails}"
            };
            Assert.That(CapturedItems.ToArray(), Is.EqualTo(expected));
        }

        [Test]
        public void NotifyScenarioFinished_should_omit_status_details_if_not_provided()
        {
            var scenarioInfo = Fake.Object<TestResults.TestScenarioInfo>();
            scenarioInfo.Labels = Array.Empty<string>();
            scenarioInfo.Description = null;
            var scenarioResult = Fake.Object<TestResults.TestScenarioResult>();
            scenarioResult.Info = scenarioInfo;
            scenarioResult.Status = ExecutionStatus.Passed;
            scenarioResult.StatusDetails = null;

            var scenarioNotifier = GetScenarioNotifier();
            scenarioNotifier.NotifyScenarioStart(scenarioInfo);
            scenarioNotifier.NotifyScenarioFinished(scenarioResult);

            var expected = new[]
            {
                $"Fi=000,Fa=000,Pe=001 #  1> SCENARIO: {scenarioInfo.Name}",
                $"Fi=001,Fa=000,Pe=000 #  1>   SCENARIO RESULT: {scenarioResult.Status} after {scenarioResult.ExecutionTime.Duration.FormatPretty()}"
            };
            Assert.That(CapturedItems.ToArray(), Is.EqualTo(expected));
        }

        [Test]
        public async Task It_should_capture_progress_properly()
        {
            var count = 100;
            var featureNotifier = GetFeatureNotifier();

            await Task.WhenAll(Enumerable.Range(0, count).Select(i => BeginNotification(i, featureNotifier)).ToArray());

            Assert.That(_capturedGroups.Keys.Count, Is.EqualTo(count), "Not all groups has been captured");

            foreach (var group in _capturedGroups.Values)
            {
                var identifiers = group.Select(v => Regex.Match(v, "^[^#]+#([^>]+)>").Groups[1].Value).Distinct().Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
                Assert.That(identifiers.Length, Is.EqualTo(1), "Expected one identifier in group, got: {0}", string.Join(", ", identifiers.Select(i => $"'{i}'")));
            }

            var finished = _capturedGroups.Values.SelectMany(v => v)
                .Select(v => Regex.Match(v, "^Fi=([^,]+),").Groups[1].Value)
                .Select(int.Parse)
                .Distinct()
                .ToArray();
            Assert.That(finished, Does.Contain(count), $"Expected at least one status with Fi={count}");
        }

        private async Task BeginNotification(int i, IFeatureProgressNotifier featureNotifier)
        {
            var scenarioNotifier = GetScenarioNotifier();

            await Task.Yield();
            _currentId.Value = i;

            featureNotifier.NotifyFeatureStart(Fake.Object<TestResults.TestFeatureInfo>());
            await Task.Yield();
            scenarioNotifier.NotifyScenarioStart(Fake.Object<TestResults.TestScenarioInfo>());
            await Task.Yield();
            scenarioNotifier.NotifyStepStart(Fake.Object<TestResults.TestStepInfo>());
            await Task.Yield();
            scenarioNotifier.NotifyStepComment(Fake.Object<TestResults.TestStepInfo>(), "comment");
            await Task.Yield();
            scenarioNotifier.NotifyStepComment(Fake.Object<TestResults.TestStepInfo>(), "comment2");
            await Task.Yield();
            scenarioNotifier.NotifyStepFinished(Fake.Object<TestResults.TestStepResult>());
            await Task.Yield();
            scenarioNotifier.NotifyStepStart(Fake.Object<TestResults.TestStepInfo>());
            await Task.Yield();
            scenarioNotifier.NotifyStepComment(Fake.Object<TestResults.TestStepInfo>(), "comment");
            await Task.Yield();
            scenarioNotifier.NotifyStepComment(Fake.Object<TestResults.TestStepInfo>(), "comment2");
            await Task.Yield();
            scenarioNotifier.NotifyStepFinished(Fake.Object<TestResults.TestStepResult>());
            await Task.Yield();

            var scenarioResult = Fake.Object<TestResults.TestScenarioResult>();
            scenarioResult.Steps = Array.Empty<TestResults.TestStepResult>();

            scenarioNotifier.NotifyScenarioFinished(scenarioResult);
            await Task.Yield();

            var featureResult = Fake.Object<TestResults.TestFeatureResult>();
            featureResult.Scenarios = Array.Empty<TestResults.TestScenarioResult>();
            featureNotifier.NotifyFeatureFinished(featureResult);
        }

    }
}