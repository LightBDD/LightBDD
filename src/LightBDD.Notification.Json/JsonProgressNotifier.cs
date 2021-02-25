using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Parameters;
using LightBDD.Core.Results.Parameters.Tabular;

namespace LightBDD.Notification.Json
{
    public class JsonProgressNotifier : IScenarioProgressNotifier, IFeatureProgressNotifier
    {
        private readonly NotificationWriter _writer;

        public JsonProgressNotifier(NotificationWriter writer)
        {
            _writer = writer;
        }

        public void NotifyScenarioStart(IScenarioInfo scenario)
        {
            _writer.Write(new ScenarioStart(scenario));
        }

        public void NotifyScenarioFinished(IScenarioResult scenario)
        {
            _writer.Write(new ScenarioFinish(scenario));
        }

        public void NotifyStepStart(IStepInfo step)
        {
            _writer.Write(new StepStart(step));
        }

        public void NotifyStepFinished(IStepResult step)
        {
            _writer.Write(new StepFinish(step));
        }

        public void NotifyStepComment(IStepInfo step, string comment)
        {
            _writer.Write(new StepComment(step, comment));
        }

        public void NotifyFeatureStart(IFeatureInfo feature)
        {
            _writer.Write(new FeatureStart(feature));
        }

        public void NotifyFeatureFinished(IFeatureResult feature)
        {
            _writer.Write(new FeatureFinish(feature));
        }
    }

    public class StepFinish : Notification
    {
        public StepFinish(IStepResult step)
        {
            RuntimeId = step.Info.RuntimeId;
            Comments = step.Comments;
            ExecutionException = ExceptionModel.From(step.ExecutionException);
            ExecutionTime = new ExecutionTimeModel(step.ExecutionTime);
            Status = step.Status;
            StatusDetails = step.StatusDetails;
            Parameters = step.Parameters.Select(ParameterResultModel.From);
        }
        [JsonPropertyName("p")]
        public IEnumerable<ParameterResultModel> Parameters { get; set; }
        [JsonPropertyName("d")]
        public string StatusDetails { get; set; }
        [JsonPropertyName("s")]
        public ExecutionStatus Status { get; set; }
        [JsonPropertyName("ex")]
        public ExceptionModel ExecutionException { get; set; }
        [JsonPropertyName("c")]
        public IEnumerable<string> Comments { get; set; }
        [JsonPropertyName("e")]
        public ExecutionTimeModel ExecutionTime { get; set; }

        protected override string GetTypeCode() => "sf";
    }

    public class ParameterResultModel
    {
        public static ParameterResultModel From(IParameterResult arg)
        {
            var model = new ParameterResultModel
            {
                Name = arg.Name
            };

            switch (arg.Details)
            {
                case IInlineParameterDetails inlineDetails:
                    model.Details = InlineParameterDetailsModel.From(inlineDetails);
                    break;
                case ITabularParameterDetails tabularDetails:
                    model.Details = TabularParameterDetailsModel.From(tabularDetails);
                    break;
            }

            return model;

        }

        [JsonPropertyName("d")]
        public object Details { get; set; }
        [JsonPropertyName("n")]
        public string Name { get; set; }
    }

    public abstract class ParameterDetailsModel
    {
        [JsonPropertyName("_c")]
        public string TypeCode => GetTypeCode();
        protected abstract string GetTypeCode();
    }

    public class TabularParameterDetailsModel : ParameterDetailsModel
    {
        public static TabularParameterDetailsModel From(ITabularParameterDetails t)
        {
            return new TabularParameterDetailsModel
            {
                VerificationStatus = t.VerificationStatus,
                VerificationMessage = t.VerificationMessage,
                Columns = t.Columns.Select(TabularParameterColumnModel.From),
                Rows = t.Rows.Select(TabularParameterRowModel.From),
            };
        }
        [JsonPropertyName("r")]
        public IEnumerable<TabularParameterRowModel> Rows { get; set; }
        [JsonPropertyName("c")]
        public IEnumerable<TabularParameterColumnModel> Columns { get; set; }
        [JsonPropertyName("m")]
        public string VerificationMessage { get; set; }
        [JsonPropertyName("s")]
        public ParameterVerificationStatus VerificationStatus { get; set; }

        protected override string GetTypeCode() => "t";
    }

    public class TabularParameterRowModel
    {
        public static TabularParameterRowModel From(ITabularParameterRow r)
        {
            return new TabularParameterRowModel
            {
                Type = r.Type,
                VerificationMessage = r.VerificationMessage,
                VerificationStatus = r.VerificationStatus,
                Values = r.Values.Select(ValueResultModel.From),
            };
        }
        [JsonPropertyName("v")]
        public IEnumerable<ValueResultModel> Values { get; set; }
        [JsonPropertyName("s")]
        public ParameterVerificationStatus VerificationStatus { get; set; }
        [JsonPropertyName("m")]
        public string VerificationMessage { get; set; }
        [JsonPropertyName("t")]
        public TableRowType Type { get; set; }
    }

    public class ValueResultModel
    {
        public static ValueResultModel From(IValueResult r)
        {
            return new ValueResultModel
            {
                Expectation = r.Expectation,
                Value = r.Value,
                VerificationMessage = r.VerificationMessage,
                VerificationStatus = r.VerificationStatus
            };
        }

        [JsonPropertyName("s")]
        public ParameterVerificationStatus VerificationStatus { get; set; }
        [JsonPropertyName("m")]
        public string VerificationMessage { get; set; }
        [JsonPropertyName("v")]
        public string Value { get; set; }
        [JsonPropertyName("e")]
        public string Expectation { get; set; }
    }

    public class TabularParameterColumnModel
    {
        public static TabularParameterColumnModel From(ITabularParameterColumn c)
        {
            return new TabularParameterColumnModel
            {
                IsKey = c.IsKey,
                Name = c.Name
            };
        }
        [JsonPropertyName("n")]
        public string Name { get; set; }
        [JsonPropertyName("k")]
        public bool IsKey { get; set; }
    }

    public class InlineParameterDetailsModel : ParameterDetailsModel
    {
        public static InlineParameterDetailsModel From(IInlineParameterDetails d)
        {
            return new InlineParameterDetailsModel
            {
                Expectation = d.Expectation,
                Value = d.Value,
                VerificationMessage = d.VerificationMessage,
                VerificationStatus = d.VerificationStatus
            };
        }

        [JsonPropertyName("s")]
        public ParameterVerificationStatus VerificationStatus { get; set; }
        [JsonPropertyName("m")]
        public string VerificationMessage { get; set; }
        [JsonPropertyName("v")]
        public string Value { get; set; }
        [JsonPropertyName("e")]
        public string Expectation { get; set; }

        protected override string GetTypeCode() => "i";
    }

    public class ExceptionModel
    {
        public static ExceptionModel From(Exception ex)
        {
            if (ex == null) return null;
            return new ExceptionModel
            {
                Message = ex.Message,
                Type = ex.GetType().FullName,
                StackTrace = ex.StackTrace,
                InnerException = ExceptionModel.From(ex.InnerException)
            };
        }

        [JsonPropertyName("i")]
        public ExceptionModel InnerException { get; set; }
        [JsonPropertyName("s")]
        public string StackTrace { get; set; }
        [JsonPropertyName("t")]
        public string Type { get; set; }
        [JsonPropertyName("m")]
        public string Message { get; set; }
    }

    public class StepStart : Notification
    {
        public StepStart(IStepInfo step)
        {
            RuntimeId = step.RuntimeId;
            GroupPrefix = step.GroupPrefix;
            Name = new StepNameModel(step.Name);
            Number = step.Number;
            ParentId = step.Parent.RuntimeId;
            Total = step.Total;
        }
        [JsonPropertyName("t")]
        public int Total { get; set; }
        [JsonPropertyName("p")]
        public Guid ParentId { get; set; }
        [JsonPropertyName("n")]
        public StepNameModel Name { get; set; }
        [JsonPropertyName("u")]
        public int Number { get; set; }
        [JsonPropertyName("g")]
        public string GroupPrefix { get; set; }

        protected override string GetTypeCode() => "ss";
    }

    public class StepNameModel : NameModel
    {
        public StepNameModel(IStepNameInfo stepName) : base(stepName)
        {
            TypeName = stepName.StepTypeName.Name;
            OriginalTypeName = stepName.StepTypeName.OriginalName;
        }
        [JsonPropertyName("o")]
        public string OriginalTypeName { get; set; }
        [JsonPropertyName("t")]
        public string TypeName { get; set; }
    }

    public class ScenarioFinish : Notification
    {
        public ScenarioFinish(IScenarioResult scenario)
        {
            RuntimeId = scenario.Info.RuntimeId;
            Status = scenario.Status;
            StatusDetails = scenario.StatusDetails;
            ExecutionTime = new ExecutionTimeModel(scenario.ExecutionTime);
        }
        [JsonPropertyName("d")]
        public string StatusDetails { get; set; }
        [JsonPropertyName("s")]
        public ExecutionStatus Status { get; set; }
        [JsonPropertyName("e")]
        public ExecutionTimeModel ExecutionTime { get; set; }

        protected override string GetTypeCode() => "Sf";
    }

    public class ExecutionTimeModel
    {
        public ExecutionTimeModel(ExecutionTime executionTime)
        {
            Start = executionTime.Start.Ticks;
            Duration = executionTime.Duration.Ticks;
        }

        [JsonPropertyName("s")]
        public long Start { get; set; }
        [JsonPropertyName("d")]
        public long Duration { get; set; }
    }

    public class ScenarioStart : Notification
    {
        public ScenarioStart(IScenarioInfo scenario)
        {
            RuntimeId = scenario.RuntimeId;
            Categories = scenario.Categories;
            Labels = scenario.Labels;
            ParentId = scenario.Parent.RuntimeId;
            Name = new NameModel(scenario.Name);
        }

        [JsonPropertyName("n")]
        public NameModel Name { get; set; }
        [JsonPropertyName("p")]
        public Guid ParentId { get; set; }
        [JsonPropertyName("l")]
        public IEnumerable<string> Labels { get; set; }
        [JsonPropertyName("c")]
        public IEnumerable<string> Categories { get; set; }

        protected override string GetTypeCode() => "Ss";
    }

    public class StepComment : Notification
    {
        public StepComment(IStepInfo step, string comment)
        {
            RuntimeId = step.RuntimeId;
            Comment = comment;
        }

        [JsonPropertyName("c")]
        public string Comment { get; set; }
        protected override string GetTypeCode() => "sc";
    }

    public class FeatureFinish : Notification
    {
        public FeatureFinish(IFeatureResult feature)
        {
            RuntimeId = feature.Info.RuntimeId;
        }
        protected override string GetTypeCode() => "ff";
    }

    public class FeatureStart : Notification
    {
        public FeatureStart() { }
        public FeatureStart(IFeatureInfo feature)
        {
            RuntimeId = feature.RuntimeId;
            Description = feature.Description;
            Labels = feature.Labels;
            Name = new NameModel(feature.Name);
        }

        [JsonPropertyName("n")]
        public NameModel Name { get; set; }

        [JsonPropertyName("l")]
        public IEnumerable<string> Labels { get; set; }

        [JsonPropertyName("d")]
        public string Description { get; set; }

        protected override string GetTypeCode() => "fs";
    }

    public class NameModel
    {
        public NameModel() { }
        public NameModel(INameInfo info)
        {
            Format = info.NameFormat;
            Parameters = info.Parameters.Select(p => new NameParameterModel(p));
        }

        [JsonPropertyName("p")]
        public IEnumerable<NameParameterModel> Parameters { get; set; }

        [JsonPropertyName("f")]
        public string Format { get; set; }
    }

    public class NameParameterModel
    {
        public NameParameterModel() { }
        public NameParameterModel(INameParameterInfo info)
        {
            IsEvaluated = info.IsEvaluated;
            FormattedValue = info.FormattedValue;
            VerificationStatus = info.VerificationStatus;
        }
        [JsonPropertyName("s")]
        public ParameterVerificationStatus VerificationStatus { get; set; }
        [JsonPropertyName("v")]
        public string FormattedValue { get; set; }
        [JsonPropertyName("e")]
        public bool IsEvaluated { get; set; }
    }
}