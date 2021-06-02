using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Notification.Events;
using LightBDD.Notification.Jsonl.Events;
using LightBDD.Notification.Jsonl.IO;
using LightBDD.Notification.Jsonl.Models;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Reporting.Progressive.UnitTests
{
    [TestFixture]
    public class JsonlProgressNotifierTests
    {
        private EventTime Time = new EventTime(DateTimeOffset.UtcNow, TimeSpan.FromMilliseconds(600));

        [Test]
        public void FeatureDiscovered_mapping()
        {
            var info = Fake.Object<TestResults.TestFeatureInfo>();
            var e = ProcessEvent<FeatureDiscoveredEvent>(new FeatureDiscovered(Time, info));

            e.Description.ShouldBe(info.Description);
            e.FeatureId.ShouldBe(info.RuntimeId);
            e.Labels.ShouldBe(info.Labels);
            AssertName(e.Name, info.Name);
            e.Time.ShouldBe(Time.Offset);
        }

        [Test]
        public void FeatureStarting_mapping()
        {
            var info = Fake.Object<TestResults.TestFeatureInfo>();
            var e = ProcessEvent<FeatureStartingEvent>(new FeatureStarting(Time, info));

            e.FeatureId.ShouldBe(info.RuntimeId);
            e.Time.ShouldBe(Time.Offset);
        }

        [Test]
        public void FeatureFinished_mapping()
        {
            var result = Fake.Object<TestResults.TestFeatureResult>();
            var e = ProcessEvent<FeatureFinishedEvent>(new FeatureFinished(Time, result));

            e.FeatureId.ShouldBe(result.Info.RuntimeId);
            e.Time.ShouldBe(Time.Offset);
        }

        [Test]
        public void TestExecutionStarting_mapping()
        {
            var e = ProcessEvent<TestExecutionStartingEvent>(new TestExecutionStarting(Time));

            e.Time.ShouldBe(Time.Offset);
            e.Start.ShouldBe(Time.Start);
        }

        [Test]
        public void TestExecutionFinished_mapping()
        {
            var result = Fake.Object<TestResults.TestExecutionResult>();
            var e = ProcessEvent<TestExecutionFinishedEvent>(new TestExecutionFinished(Time, result));
            e.Time.ShouldBe(Time.Offset);
        }

        [Test]
        public void ScenarioDiscovered_mapping()
        {
            var info = Fake.Object<TestResults.TestScenarioInfo>();
            var e = ProcessEvent<ScenarioDiscoveredEvent>(new ScenarioDiscovered(Time, info));

            e.ParentId.ShouldBe(info.Parent.RuntimeId);
            e.ScenarioId.ShouldBe(info.RuntimeId);
            e.Categories.ShouldBe(info.Categories);
            e.Labels.ShouldBe(info.Labels);
            AssertName(e.Name, info.Name);
            e.Time.ShouldBe(Time.Offset);
        }

        [Test]
        public void ScenarioStarting_mapping()
        {
            var info = Fake.Object<TestResults.TestScenarioInfo>();
            var e = ProcessEvent<ScenarioStartingEvent>(new ScenarioStarting(Time, info));

            e.Time.ShouldBe(Time.Offset);
            e.ScenarioId.ShouldBe(info.RuntimeId);
        }

        [Test]
        public void ScenarioFinished_mapping()
        {
            var result = Fake.Object<TestResults.TestScenarioResult>();
            var e = ProcessEvent<ScenarioFinishedEvent>(new ScenarioFinished(Time, result));

            e.ScenarioId.ShouldBe(result.Info.RuntimeId);
            e.Time.ShouldBe(Time.Offset);
            e.Status.ShouldBe((ExecutionStatus)result.Status);
            e.StatusDetails.ShouldBe(result.StatusDetails);
        }

        [Test]
        public void StepDiscovered_mapping()
        {
            var info = Fake.Object<TestResults.TestStepInfo>();
            var e = ProcessEvent<StepDiscoveredEvent>(new StepDiscovered(Time, info));

            e.ParentId.ShouldBe(info.Parent.RuntimeId);
            e.StepId.ShouldBe(info.RuntimeId);
            e.GroupPrefix.ShouldBe(info.GroupPrefix);
            e.Number.ShouldBe(info.Number);
            AssertName(e.Name, info.Name);
            e.Time.ShouldBe(Time.Offset);
        }

        [Test]
        public void StepStarting_mapping()
        {
            var info = Fake.Object<TestResults.TestStepInfo>();
            var e = ProcessEvent<StepStartingEvent>(new StepStarting(Time, info));

            e.Time.ShouldBe(Time.Offset);
            e.StepId.ShouldBe(info.RuntimeId);
        }

        [Test]
        public void StepFinished_mapping()
        {
            var result = Fake.Object<TestResults.TestStepResult>();
            result.ExecutionException = new InvalidOperationException("foo", new Exception("bar"));

            var e = ProcessEvent<StepFinishedEvent>(new StepFinished(Time, result));

            e.StepId.ShouldBe(result.Info.RuntimeId);
            e.Time.ShouldBe(Time.Offset);
            e.Status.ShouldBe((ExecutionStatus)result.Status);
            e.StatusDetails.ShouldBe(result.StatusDetails);
            AssertException(e.Exception, result.ExecutionException);
        }

        [Test]
        public void StepCommented_mapping()
        {
            var info = Fake.Object<TestResults.TestStepInfo>();
            var e = ProcessEvent<StepCommentedEvent>(new StepCommented(Time, info, "comment"));

            e.Time.ShouldBe(Time.Offset);
            e.StepId.ShouldBe(info.RuntimeId);
            e.Comment.ShouldBe("comment");
        }

        private void AssertException(ExceptionModel actual, Exception expected)
        {
            actual.Type.ShouldBe(expected.GetType().FullName);
            actual.Message.ShouldBe(expected.Message);
            actual.StackTrace.ShouldBe(expected.StackTrace);
            if (expected.InnerException != null)
                AssertException(actual.InnerException, expected.InnerException);
        }

        private void AssertName(NameModel actual, TestResults.TestNameInfo expected)
        {
            actual.Format.ShouldBe(expected.NameFormat);
            for (int i = 0; i < expected.Parameters.Length; i++)
                AssertParameter(actual.Parameters[i], expected.Parameters[i]);
        }

        private void AssertName(StepNameModel actual, TestResults.TestStepNameInfo expected)
        {
            actual.TypeName.ShouldBe(expected.StepTypeName.Name);
            actual.OriginalTypeName.ShouldBe(expected.StepTypeName.OriginalName);
            actual.Format.ShouldBe(expected.NameFormat);
            for (int i = 0; i < expected.Parameters.Length; i++)
                AssertParameter(actual.Parameters[i], expected.Parameters[i]);
        }

        private void AssertParameter(NameParameterModel actual, TestResults.TestNameParameterInfo expected)
        {
            actual.IsEvaluated.ShouldBe(expected.IsEvaluated);
            actual.FormattedValue.ShouldBe(expected.FormattedValue);
            actual.VerificationStatus.ShouldBe((ParameterVerificationStatus)expected.VerificationStatus);
        }

        private static T ProcessEvent<T>(ProgressEvent progressEvent) where T : NotificationEvent
        {
            var serializer = new JsonlEventSerializer();
            var queue = new ConcurrentQueue<NotificationEvent>();

            Task OnCapture(string e)
            {
                queue.Enqueue(serializer.Deserialize(e));
                return Task.CompletedTask;
            }

            using (var notifier = new JsonlProgressNotifier(OnCapture))
                notifier.Notify(progressEvent);

            return queue.Cast<T>().Single();
        }
    }
}
