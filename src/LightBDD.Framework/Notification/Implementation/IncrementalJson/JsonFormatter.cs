using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results;

namespace LightBDD.Framework.Notification.Implementation.IncrementalJson
{
    internal static class JsonFormatter
    {
        public static JsonElement ToJson(this IFeatureInfo value, Stopwatch timeLine)
        {
            return new JsonElement(
                ToType("FeatureStarted"),
                ToTimeLine(timeLine),
                new JsonProperty(nameof(value.RuntimeId), ToJson(value.RuntimeId)),
                new JsonProperty(nameof(value.Name), ToJson(value.Name)),
                new JsonProperty(nameof(value.Labels), value.Labels.ToJsonArray(ToJson)),
                new JsonProperty(nameof(value.Description), value.Description)
            );
        }

        public static JsonElement ToJson(this IScenarioInfo value, Stopwatch timeLine)
        {
            return new JsonElement(
                ToType("ScenarioStarted"),
                ToTimeLine(timeLine),
                new JsonProperty(nameof(value.RuntimeId), ToJson(value.RuntimeId)),
                new JsonProperty(nameof(value.Name), ToJson(value.Name)),
                new JsonProperty(nameof(value.Labels), value.Labels.ToJsonArray(ToJson)),
                new JsonProperty(nameof(value.Categories), value.Categories.ToJsonArray(ToJson))
            );
        }

        public static JsonElement ToJson(this IScenarioResult value, Stopwatch timeLine)
        {
            return new JsonElement(
                ToType("ScenarioFinished"),
                ToTimeLine(timeLine),
                new JsonProperty(nameof(value.Info.RuntimeId), ToJson(value.Info.RuntimeId)),
                new JsonProperty(nameof(value.Status), value.Status.ToString()),
                new JsonProperty(nameof(value.StatusDetails), value.StatusDetails),
                new JsonProperty(nameof(value.ExecutionTime), ToJson(value.ExecutionTime))
            );
        }

        public static JsonElement ToJson(this IStepResult value, Stopwatch timeLine)
        {
            return new JsonElement(
                ToType("StepFinished"),
                ToTimeLine(timeLine),
                new JsonProperty(nameof(value.Info.RuntimeId), ToJson(value.Info.RuntimeId)),
                new JsonProperty(nameof(value.Status), value.Status.ToString()),
                new JsonProperty(nameof(value.StatusDetails), value.StatusDetails),
                new JsonProperty(nameof(value.ExecutionTime), ToJson(value.ExecutionTime))
            );
        }

        public static JsonElement ToStepCommentJson(this IStepInfo value, Stopwatch timeLine, string comment)
        {
            return new JsonElement(
                ToType("StepComment"),
                ToTimeLine(timeLine),
                new JsonProperty(nameof(value.RuntimeId), ToJson(value.RuntimeId)),
                new JsonProperty("Comment", comment)
            );
        }

        private static JsonElement ToJson(ExecutionTime executionTime)
        {
            return new JsonElement(
                new JsonProperty(nameof(executionTime.Start), executionTime.Start.ToString("s")),
                new JsonProperty(nameof(executionTime.End), executionTime.End.ToString("s")),
                new JsonProperty(nameof(executionTime.Duration), executionTime.Duration.TotalMilliseconds)
            );
        }

        private static JsonElement ToJson(INameInfo name)
        {
            return new JsonElement(
                new JsonProperty(nameof(name.NameFormat), name.NameFormat),
                new JsonProperty(nameof(name.Parameters), name.Parameters.ToJsonArray(ToJson)));
        }

        private static JsonElement ToJson(IStepNameInfo name)
        {
            return new JsonElement(
                new JsonProperty(nameof(name.StepTypeName), ToJson(name.StepTypeName)),
                new JsonProperty(nameof(name.NameFormat), name.NameFormat),
                new JsonProperty(nameof(name.Parameters), name.Parameters.ToJsonArray(ToJson)));
        }

        private static JsonElement ToJson(IStepTypeNameInfo stepTypeName)
        {
            return new JsonElement(
                new JsonProperty(nameof(stepTypeName.Name), stepTypeName.Name),
                new JsonProperty(nameof(stepTypeName.OriginalName), stepTypeName.OriginalName));
        }

        private static IJsonItem ToJson(INameParameterInfo paramInfo)
        {
            return new JsonElement(
                new JsonProperty(nameof(paramInfo.FormattedValue), paramInfo.FormattedValue),
                new JsonProperty(nameof(paramInfo.IsEvaluated), paramInfo.IsEvaluated));
        }

        public static JsonElement ToJson(this IStepInfo value, Stopwatch timeLine)
        {
            return new JsonElement(
                ToType("StepStarted"),
                ToTimeLine(timeLine),
                new JsonProperty(nameof(value.RuntimeId), ToJson(value.RuntimeId)),
                new JsonProperty(nameof(value.Name), ToJson(value.Name)),
                new JsonProperty(nameof(value.GroupPrefix), value.GroupPrefix),
                new JsonProperty(nameof(value.Number), value.Number),
                new JsonProperty(nameof(value.Total), value.Total)
            );
        }

        public static JsonElement ToTestsFinished(Stopwatch timeLine)
        {
            return new JsonElement(
                ToType("TestsFinished"),
                ToTimeLine(timeLine));
        }

        public static JsonString ToJson(string value) => new JsonString(value);

        public static JsonElement ToJson(this IFeatureResult value, Stopwatch timeLine)
        {
            return new JsonElement(
                ToType("FeatureFinished"),
                ToTimeLine(timeLine),
                new JsonProperty(nameof(value.Info.RuntimeId), ToJson(value.Info.RuntimeId)));
        }

        public static JsonArray ToJsonArray<T>(this IEnumerable<T> values, Func<T, IJsonItem> formatter)
        {
            return new JsonArray(values.Select(formatter));
        }

        private static JsonProperty ToType(string type) => new JsonProperty("t", type);

        private static JsonProperty ToTimeLine(Stopwatch timeLine) =>
            new JsonProperty("l", timeLine.ElapsedMilliseconds);
    }
}