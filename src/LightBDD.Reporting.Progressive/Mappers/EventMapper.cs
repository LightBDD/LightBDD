using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification.Events;
using LightBDD.Notification.Jsonl.Events;
using LightBDD.Notification.Jsonl.Models;
using FeatureFinished = LightBDD.Core.Notification.Events.FeatureFinished;
using FeatureStarting = LightBDD.Core.Notification.Events.FeatureStarting;
using ParameterVerificationStatus = LightBDD.Notification.Jsonl.Models.ParameterVerificationStatus;
using ScenarioFinished = LightBDD.Core.Notification.Events.ScenarioFinished;
using ScenarioStarting = LightBDD.Core.Notification.Events.ScenarioStarting;
using StepCommented = LightBDD.Core.Notification.Events.StepCommented;
using StepFinished = LightBDD.Core.Notification.Events.StepFinished;
using StepStarting = LightBDD.Core.Notification.Events.StepStarting;

namespace LightBDD.Reporting.Progressive.Mappers
{
    internal static class EventMapper
    {
        public static FeatureDiscovered ToFeatureDiscovered(this FeatureStarting src)
        {
            return new FeatureDiscovered
            {
                Description = src.Feature.Description,
                Time = src.Time.Offset,
                Name = src.Feature.Name.ToJsonlModel(),
                Id = src.Feature.RuntimeId,
                Labels = src.Feature.Labels.ToArray()
            };
        }

        public static Notification.Jsonl.Events.FeatureStarting ToFeatureStarting(this FeatureStarting src)
        {
            return new Notification.Jsonl.Events.FeatureStarting
            {
                Time = src.Time.Offset,
                Id = src.Feature.RuntimeId
            };
        }

        public static Notification.Jsonl.Events.FeatureFinished ToFeatureFinished(FeatureFinished e)
        {
            return new Notification.Jsonl.Events.FeatureFinished
            {
                Id = e.Result.Info.RuntimeId,
                Time = e.Time.Offset,
                Status = (ExecutionStatus)e.Result.GetScenarios().Select(s => s.Status).DefaultIfEmpty().Max()
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

        public static Notification.Jsonl.Events.ScenarioStarting ToScenarioStarting(ScenarioStarting e)
        {
            return new Notification.Jsonl.Events.ScenarioStarting
            {
                Id = e.Scenario.RuntimeId,
                Time = e.Time.Offset
            };
        }

        public static ScenarioDiscovered ToScenarioDiscovered(ScenarioStarting e)
        {
            return new ScenarioDiscovered
            {
                Time = e.Time.Offset,
                Id = e.Scenario.RuntimeId,
                Categories = e.Scenario.Categories.ToArray(),
                Labels = e.Scenario.Labels.ToArray(),
                Name = e.Scenario.Name.ToJsonlModel(),
                ParentId = e.Scenario.Parent.RuntimeId
            };
        }

        public static Notification.Jsonl.Events.ScenarioFinished ToScenarioFinished(ScenarioFinished e)
        {
            return new Notification.Jsonl.Events.ScenarioFinished
            {
                Time = e.Time.Offset,
                Id = e.Result.Info.RuntimeId,
                Status = (ExecutionStatus)e.Result.Status,
                StatusDetails = e.Result.StatusDetails
            };
        }

        public static Notification.Jsonl.Events.StepCommented ToStepCommented(StepCommented e)
        {
            return new Notification.Jsonl.Events.StepCommented
            {
                Comment = e.Comment,
                Time = e.Time.Offset,
                StepId = e.Step.RuntimeId
            };
        }

        public static Notification.Jsonl.Events.StepFinished ToStepFinished(StepFinished e)
        {
            return new Notification.Jsonl.Events.StepFinished
            {
                Time = e.Time.Offset,
                Id = e.Result.Info.RuntimeId,
                Status = (ExecutionStatus)e.Result.Status,
                StatusDetails = e.Result.StatusDetails,
                ExecutionException = e.Result.ExecutionException.ToJsonlModel()
            };
        }

        public static StepDiscovered ToStepDiscovered(StepStarting e)
        {
            return new StepDiscovered
            {
                Time = e.Time.Offset,
                Id = e.Step.RuntimeId,
                GroupPrefix = e.Step.GroupPrefix,
                Name = e.Step.Name.ToJsonlModel(),
                ParentId = e.Step.Parent.RuntimeId,
                Number = e.Step.Number
            };
        }

        public static Notification.Jsonl.Events.StepStarting ToStepStarting(StepStarting e)
        {
            return new Notification.Jsonl.Events.StepStarting
            {
                Id = e.Step.RuntimeId,
                Time = e.Time.Offset
            };
        }
    }
}
