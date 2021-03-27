using System;
using System.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification.Events;
using LightBDD.Notification.Jsonl.Models;
using LightBDD.Notification.Jsonl.Events;
using ParameterVerificationStatus = LightBDD.Notification.Jsonl.Models.ParameterVerificationStatus;

namespace LightBDD.Reporting.Progressive.Mappers
{
    internal static class EventMapper
    {
        public static FeatureDiscoveredEvent ToFeatureDiscovered(this FeatureStarting src)
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

        public static FeatureStartingEvent ToFeatureStarting(this FeatureStarting src)
        {
            return new FeatureStartingEvent
            {
                Time = src.Time.Offset,
                FeatureId = src.Feature.RuntimeId
            };
        }

        public static FeatureFinishedEvent ToFeatureFinished(FeatureFinished e)
        {
            return new FeatureFinishedEvent
            {
                FeatureId = e.Result.Info.RuntimeId,
                Time = e.Time.Offset
            };
        }

        public static NameModel ToJsonlModel(this INameInfo info)
        {
            return new NameModel
            {
                Format = info.NameFormat,
                Parameters = info.Parameters.Select(ToJsonlModel).ToArray()
            };
        }

        public static StepNameModel ToJsonlModel(this IStepNameInfo info)
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

        public static ScenarioStartingEvent ToScenarioStarting(ScenarioStarting e)
        {
            return new ScenarioStartingEvent
            {
                ScenarioId = e.Scenario.RuntimeId,
                Time = e.Time.Offset
            };
        }

        public static ScenarioDiscoveredEvent ToScenarioDiscovered(ScenarioStarting e)
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

        public static ScenarioFinishedEvent ToScenarioFinished(ScenarioFinished e)
        {
            return new ScenarioFinishedEvent
            {
                Time = e.Time.Offset,
                ScenarioId = e.Result.Info.RuntimeId,
                Status = (ExecutionStatus)e.Result.Status,
                StatusDetails = e.Result.StatusDetails
            };
        }

        public static StepCommentedEvent ToStepCommented(StepCommented e)
        {
            return new StepCommentedEvent
            {
                Comment = e.Comment,
                Time = e.Time.Offset,
                StepId = e.Step.RuntimeId
            };
        }

        public static StepFinishedEvent ToStepFinished(StepFinished e)
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

        public static StepDiscoveredEvent ToStepDiscovered(StepStarting e)
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

        public static StepStartingEvent ToStepStarting(StepStarting e)
        {
            return new StepStartingEvent
            {
                StepId = e.Step.RuntimeId,
                Time = e.Time.Offset
            };
        }
    }
}
