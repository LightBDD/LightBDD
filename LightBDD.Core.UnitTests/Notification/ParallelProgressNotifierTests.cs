using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Execution.Results;
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
            public ConcurrentDictionary<int, ConcurrentQueue<string>> Captured { get; } = new ConcurrentDictionary<int, ConcurrentQueue<string>>();
            public ConcurrentQueue<string> CapturedQueue { get; } = new ConcurrentQueue<string>();

            public CapturingParallelProgressNotifier(ProgressManager progressManager) : base(progressManager)
            {
            }

            protected override void Notify(string message)
            {
                Captured.GetOrAdd(_id.Value, i => new ConcurrentQueue<string>()).Enqueue(message);
                CapturedQueue.Enqueue(message);
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
        public async Task It_should_capture_progress_properly()
        {
            var count = 1000;
            await Task.WhenAll(Enumerable.Range(0, count).Select(BeginNotification).ToArray());

            Assert.That(_notifier.Captured.Keys.Count, Is.EqualTo(count), "Not all groups has been captured");

            foreach (var group in _notifier.Captured.Values)
            {
                var identifiers = group.Select(v => Regex.Match(v, "^[^#]+#([^>]+)>").Groups[1].Value).Distinct().Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
                Assert.That(identifiers.Length, Is.EqualTo(1), "Expected one identifier in group, got: {0}", string.Join(", ", identifiers.Select(i => $"'{i}'")));
            }

            var finished = _notifier.Captured.Values.SelectMany(v => v)
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