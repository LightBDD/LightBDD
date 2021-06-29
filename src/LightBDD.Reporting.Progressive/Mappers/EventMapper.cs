using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification.Events;
using LightBDD.Framework.Notification.Events;
using LightBDD.Notification.Jsonl.Models;
using LightBDD.Notification.Jsonl.Events;
using ParameterVerificationStatus = LightBDD.Notification.Jsonl.Models.ParameterVerificationStatus;

namespace LightBDD.Reporting.Progressive.Mappers
{
    internal static class EventMapper
    {
        private static readonly Dictionary<Type, List<Func<ProgressEvent, NotificationEvent>>> Mappers = new Dictionary<Type, List<Func<ProgressEvent, NotificationEvent>>>();

        private static void Register<TEvent>(Func<TEvent, NotificationEvent> mapFn) where TEvent : ProgressEvent
        {
            if (!Mappers.TryGetValue(typeof(TEvent), out var list))
                Mappers.Add(typeof(TEvent), list = new List<Func<ProgressEvent, NotificationEvent>>());
            list.Add(e => mapFn.Invoke((TEvent)e));
        }

        static EventMapper()
        {
            Register<FeatureDiscovered>(ToFeatureDiscovered);
            Register<FeatureStarting>(ToFeatureStarting);
            Register<FeatureFinished>(ToFeatureFinished);
            Register<ScenarioStarting>(ToScenarioStarting);
            Register<ScenarioDiscovered>(ToScenarioDiscovered);
            Register<ScenarioFinished>(ToScenarioFinished);
            Register<StepCommented>(ToStepCommented);
            Register<StepDiscovered>(ToStepDiscovered);
            Register<StepStarting>(ToStepStarting);
            Register<StepFinished>(ToStepFinished);
            Register<TestExecutionStarting>(ToTestExecutionStarting);
            Register<TestExecutionFinished>(ToTestExecutionFinished);
            Register<InlineParameterDiscovered>(ToInlineParameterDiscovered);
            Register<InlineParameterValidationStarting>(ToInlineParameterValidationStarting);
            Register<InlineParameterValidationFinished>(ToInlineParameterValidationFinished);
        }

        private static InlineParameterValidationFinishedEvent ToInlineParameterValidationFinished(InlineParameterValidationFinished src)
        {
            return new InlineParameterValidationFinishedEvent
            {
                Time = src.Time.Offset,
                ParameterId = src.Parameter.RuntimeId,
                Expectation = src.Details.Expectation,
                VerificationStatus = (ParameterVerificationStatus)src.Details.VerificationStatus,
                Value = src.Details.Value,
                VerificationMessage = src.Details.VerificationMessage
            };
        }

        private static InlineParameterValidationStartingEvent ToInlineParameterValidationStarting(InlineParameterValidationStarting src)
        {
            return new InlineParameterValidationStartingEvent
            {
                Time = src.Time.Offset,
                ParameterId = src.Parameter.RuntimeId
            };
        }

        private static InlineParameterDiscoveredEvent ToInlineParameterDiscovered(InlineParameterDiscovered src)
        {
            return new InlineParameterDiscoveredEvent
            {
                Time = src.Time.Offset,
                ParentId = src.Parameter.Owner.RuntimeId,
                ParameterId = src.Parameter.RuntimeId,
                Name = src.Parameter.Name,
                Expectation = src.Details.Expectation
            };
        }

        private static TestExecutionFinishedEvent ToTestExecutionFinished(TestExecutionFinished src)
        {
            return new TestExecutionFinishedEvent
            {
                Time = src.Time.Offset
            };
        }

        public static IEnumerable<NotificationEvent> Map(ProgressEvent e)
        {
            return Mappers.TryGetValue(e.GetType(), out var fns)
                ? fns.Select(f => f.Invoke(e))
                : Enumerable.Empty<NotificationEvent>();
        }

        private static TestExecutionStartingEvent ToTestExecutionStarting(TestExecutionStarting src)
        {
            return new TestExecutionStartingEvent
            {
                Start = src.Time.Start,
                Time = src.Time.Offset
            };
        }

        private static FeatureDiscoveredEvent ToFeatureDiscovered(this FeatureDiscovered src)
        {
            return new FeatureDiscoveredEvent
            {
                Description = src.Feature.Description,
                Time = src.Time.Offset,
                Name = src.Feature.Name.ToJsonlModel(),
                FeatureId = src.Feature.RuntimeId,
                Labels = src.Feature.Labels.ToArray()
            };
        }

        private static FeatureStartingEvent ToFeatureStarting(this FeatureStarting src)
        {
            return new FeatureStartingEvent
            {
                Time = src.Time.Offset,
                FeatureId = src.Feature.RuntimeId
            };
        }

        private static FeatureFinishedEvent ToFeatureFinished(FeatureFinished e)
        {
            return new FeatureFinishedEvent
            {
                FeatureId = e.Result.Info.RuntimeId,
                Time = e.Time.Offset
            };
        }

        private static NameModel ToJsonlModel(this INameInfo info)
        {
            return new NameModel
            {
                Format = info.NameFormat,
                Parameters = info.Parameters.Select(ToJsonlModel).ToArray()
            };
        }

        private static StepNameModel ToJsonlModel(this IStepNameInfo info)
        {
            return new StepNameModel
            {
                Format = info.NameFormat,
                Parameters = info.Parameters.Select(ToJsonlModel).ToArray(),
                OriginalTypeName = info.StepTypeName.OriginalName,
                TypeName = info.StepTypeName.Name
            };
        }

        private static NameParameterModel ToJsonlModel(INameParameterInfo info)
        {
            return new NameParameterModel
            {
                FormattedValue = info.FormattedValue,
                IsEvaluated = info.IsEvaluated,
                VerificationStatus = (ParameterVerificationStatus)info.VerificationStatus
            };
        }
        private static ExceptionModel ToJsonlModel(this Exception ex)
        {
            if (ex == null)
                return null;
            return new ExceptionModel
            {
                Message = ex.Message,
                Type = ex.GetType().FullName,
                StackTrace = ex.StackTrace,
                InnerException = ToJsonlModel(ex.InnerException)
            };
        }

        private static ScenarioStartingEvent ToScenarioStarting(ScenarioStarting e)
        {
            return new ScenarioStartingEvent
            {
                ScenarioId = e.Scenario.RuntimeId,
                Time = e.Time.Offset
            };
        }

        private static ScenarioDiscoveredEvent ToScenarioDiscovered(ScenarioDiscovered e)
        {
            return new ScenarioDiscoveredEvent
            {
                Time = e.Time.Offset,
                ScenarioId = e.Scenario.RuntimeId,
                Categories = e.Scenario.Categories.ToArray(),
                Labels = e.Scenario.Labels.ToArray(),
                Name = e.Scenario.Name.ToJsonlModel(),
                ParentId = e.Scenario.Parent.RuntimeId
            };
        }

        private static ScenarioFinishedEvent ToScenarioFinished(ScenarioFinished e)
        {
            return new ScenarioFinishedEvent
            {
                Time = e.Time.Offset,
                ScenarioId = e.Result.Info.RuntimeId,
                Status = (ExecutionStatus)e.Result.Status,
                StatusDetails = e.Result.StatusDetails
            };
        }

        private static StepCommentedEvent ToStepCommented(StepCommented e)
        {
            return new StepCommentedEvent
            {
                Comment = e.Comment,
                Time = e.Time.Offset,
                StepId = e.Step.RuntimeId
            };
        }

        private static StepFinishedEvent ToStepFinished(StepFinished e)
        {
            return new StepFinishedEvent
            {
                Time = e.Time.Offset,
                StepId = e.Result.Info.RuntimeId,
                Status = (ExecutionStatus)e.Result.Status,
                StatusDetails = e.Result.StatusDetails,
                Exception = e.Result.ExecutionException.ToJsonlModel()
            };
        }

        private static StepDiscoveredEvent ToStepDiscovered(StepDiscovered e)
        {
            return new StepDiscoveredEvent
            {
                Time = e.Time.Offset,
                StepId = e.Step.RuntimeId,
                GroupPrefix = e.Step.GroupPrefix,
                Name = e.Step.Name.ToJsonlModel(),
                ParentId = e.Step.Parent.RuntimeId,
                Number = e.Step.Number
            };
        }

        private static StepStartingEvent ToStepStarting(StepStarting e)
        {
            return new StepStartingEvent
            {
                StepId = e.Step.RuntimeId,
                Time = e.Time.Offset
            };
        }
    }
}
