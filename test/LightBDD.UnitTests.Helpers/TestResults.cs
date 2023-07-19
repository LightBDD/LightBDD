using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Formatting.NameDecorators;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Parameters;
using LightBDD.Core.Results.Parameters.Tabular;
using LightBDD.Framework;
using LightBDD.Framework.Reporting;

namespace LightBDD.UnitTests.Helpers
{
    public static class TestResults
    {
        public static TestStepResult CreateStepResult(int stepNumber, string stepName, ExecutionStatus status, DateTimeOffset executionStart, TimeSpan executionTime, string statusDetails = null)
        {
            return CreateStepResult(stepNumber, stepName, status)
                .WithDetails(statusDetails)
                .WithExecutionTime(executionStart, executionTime);
        }

        public static TestStepResult CreateStepResult(int stepNumber, string stepName, ExecutionStatus status)
        {
            return CreateStepResult(status)
                .WithStepNameDetails(stepNumber, stepName, stepName)
                .WithComments();
        }

        public static TestStepResult CreateStepResult(ExecutionStatus status)
        {
            return new TestStepResult { Status = status };
        }

        public static TestStepResult WithComments(this TestStepResult result, params string[] comments)
        {
            result.Comments = comments;
            return result;
        }

        public static TestStepResult WithAttachment(this TestStepResult result, params FileAttachment[] fileAttachments)
        {
            result.FileAttachments = fileAttachments;
            return result;
        }

        public static TestStepResult WithStepNameDetails(this TestStepResult result, int stepNumber, string stepName, string nameFormat, string stepTypeName = null, params string[] parameters)
        {
            result.Info = new TestStepInfo
            {
                Name = CreateStepName(stepName, stepTypeName, nameFormat, parameters),
                Number = stepNumber
            };
            return result;
        }

        public static TestStepResult WithDetails(this TestStepResult result, string statusDetails)
        {
            result.StatusDetails = statusDetails;
            return result;
        }

        public static TestStepResult WithExecutionTime(this TestStepResult result, DateTimeOffset executionStart, TimeSpan executionTime)
        {
            result.ExecutionTime = new TestExecutionTime { Start = executionStart, Duration = executionTime };
            return result;
        }

        public static TestStepResult WithStepParameters(this TestStepResult result, params IParameterResult[] parameters)
        {
            result.Parameters = parameters;
            return result;
        }

        public static TestStepResult WithSubSteps(this TestStepResult result, params TestStepResult[] subSteps)
        {
            result.SubSteps = subSteps;
            return result;
        }

        public static TestStepResult WithRuntimeId(this TestStepResult result, Guid id)
        {
            result.Info.RuntimeId = id.ToString();
            return result;
        }

        public static TestStepResult WithGroupPrefix(this TestStepResult result, string groupPrefix)
        {
            result.Info.GroupPrefix = groupPrefix;
            return result;
        }

        public static TestStepNameInfo CreateStepName(string stepName, string stepTypeName, string nameFormat, params string[] parameters)
        {
            return new TestStepNameInfo
            {
                FormattedName = stepName,
                StepTypeName = stepTypeName != null ? new TestStepTypeNameInfo { Name = stepTypeName, OriginalName = stepTypeName } : null,
                NameFormat = nameFormat,
                Parameters = parameters.Select(CreateStepNameParameter).ToArray()
            };
        }

        private static TestNameParameterInfo CreateStepNameParameter(string parameter)
        {
            return new TestNameParameterInfo
            {
                FormattedValue = parameter,
                IsEvaluated = true
            };
        }

        public static TestScenarioResult CreateScenarioResult(string name, string label, DateTimeOffset executionStart, TimeSpan executionTime, string[] categories, params TestStepResult[] steps)
        {
            return CreateScenarioResult(CreateNameInfo(name), label, executionStart, executionTime, categories, steps);
        }

        public static TestScenarioResult CreateScenarioResult(TestNameInfo name, string label, DateTimeOffset executionStart, TimeSpan executionTime, string[] categories, params TestStepResult[] steps)
        {
            var scenarioInfo = CreateScenarioInfo(name, label, categories);
            foreach (var step in steps)
                step.Info.Parent = scenarioInfo;

            return new TestScenarioResult
            {
                Info = scenarioInfo,
                Steps = steps,
                ExecutionTime = new TestExecutionTime { Start = executionStart, Duration = executionTime },
                Status = steps.Max(s => s.Status),
                StatusDetails = string.Join(Environment.NewLine, steps.Where(s => s.StatusDetails != null).Select(s => $"Step {s.Info.Number}: {s.StatusDetails.Trim().Replace(Environment.NewLine, Environment.NewLine + "\t")}"))
            };
        }

        private static TestScenarioInfo CreateScenarioInfo(TestNameInfo name, string label, string[] categories)
        {
            return new TestScenarioInfo
            {
                Name = name,
                Labels = label != null ? new[] { label } : Array.Empty<string>(),
                Categories = categories ?? Array.Empty<string>()
            };
        }

        public static TestNameInfo CreateNameInfo(string name, string nameFormat = null, params string[] parameters)
        {
            return new TestNameInfo
            {
                FormattedName = name,
                NameFormat = nameFormat ?? name,
                Parameters = parameters.Select(CreateStepNameParameter).ToArray()
            };
        }

        public static TestFeatureResult CreateFeatureResult(string name, string description, string label, params TestScenarioResult[] scenarios)
        {
            var featureInfo = CreateFeatureInfo(name, description, label);
            foreach (var scenario in scenarios)
                scenario.Info.Parent = featureInfo;

            return new TestFeatureResult
            {
                Info = featureInfo,
                Scenarios = scenarios
            };
        }

        public static IParameterResult CreateTestParameter(string parameter, IParameterDetails result)
        {
            return new TestParameterResult(parameter, result);
        }

        public static TestInlineParameterDetails CreateInlineParameterDetails(string value)
        {
            return new TestInlineParameterDetails(value);
        }

        public static TestTabularParameterDetails CreateTabularParameterDetails(ParameterVerificationStatus status)
        {
            return new TestTabularParameterDetails(status);
        }

        public static TestTabularParameterDetails WithKeyColumns(this TestTabularParameterDetails details, params string[] columns)
        {
            details.Columns.AddRange(columns.Select(x => new TestTabularParameterColumn(true, x)));
            return details;
        }

        public static TestTabularParameterDetails WithValueColumns(this TestTabularParameterDetails details, params string[] columns)
        {
            details.Columns.AddRange(columns.Select(x => new TestTabularParameterColumn(false, x)));
            return details;
        }

        public static TestTabularParameterDetails AddRow(this TestTabularParameterDetails details, TableRowType type, ParameterVerificationStatus status, params TestValueResult[] values)
        {
            details.Rows.Add(new TestTabularParameterRow(type, status, values));
            return details;
        }

        public static TestValueResult CreateValueResult(string value)
        {
            return new TestValueResult { Value = value, VerificationStatus = ParameterVerificationStatus.NotApplicable };
        }

        public static TestValueResult CreateValueResult(string expected, string value, ParameterVerificationStatus status)
        {
            return new TestValueResult { Value = value, VerificationStatus = status, Expectation = expected };
        }

        private static TestFeatureInfo CreateFeatureInfo(string name, string description, string label)
        {
            return new TestFeatureInfo
            {
                Name = CreateNameInfo(name),
                Description = description,
                Labels = label != null ? new[] { label } : Array.Empty<string>()
            };
        }

        #region Mockable objects
        public class TestNameInfo : INameInfo
        {
            public string NameFormat { get; set; }
            IEnumerable<INameParameterInfo> INameInfo.Parameters => Parameters;
            public TestNameParameterInfo[] Parameters { get; set; }

            public string FormattedName { get; set; }
            public string Format(INameDecorator decorator)
            {
                return FormattedName;
            }

            public override string ToString()
            {
                return Format(StepNameDecorators.Default);
            }
        }

        public class TestStepNameInfo : IStepNameInfo
        {
            public string FormattedName { get; set; }
            public string NameFormat { get; set; }
            IEnumerable<INameParameterInfo> INameInfo.Parameters => Parameters;
            public TestNameParameterInfo[] Parameters { get; set; }
            public TestStepTypeNameInfo StepTypeName { get; set; }
            IStepTypeNameInfo IStepNameInfo.StepTypeName => StepTypeName;

            //TODO: rework to make it testing real code
            public string Format(IStepNameDecorator decorator)
            {
                return StepTypeName != null
                    ? $"{decorator.DecorateStepTypeName(StepTypeName)} {Format((INameDecorator)decorator)}"
                    : Format((INameDecorator)decorator);
            }

            //TODO: rework to make it testing real code
            public string Format(INameDecorator decorator)
            {
                return !Parameters.Any()
                    ? decorator.DecorateNameFormat(NameFormat)
                    : string.Format(decorator.DecorateNameFormat(NameFormat), Parameters.Select(p => (object)decorator.DecorateParameterValue(p)).ToArray());
            }

            public override string ToString()
            {
                return Format(StepNameDecorators.Default);
            }
        }

        public class TestStepResult : IStepResult
        {
            IStepInfo IStepResult.Info => Info;
            public TestStepInfo Info { get; set; }
            public ExecutionStatus Status { get; set; }
            public string StatusDetails { get; set; }
            public TestExecutionTime ExecutionTime { get; set; }
            ExecutionTime IStepResult.ExecutionTime => ExecutionTime?.ToMockedType();
            IEnumerable<string> IStepResult.Comments => Comments;
            public Exception ExecutionException { get; }
            IReadOnlyList<IParameterResult> IStepResult.Parameters => Parameters;

            public IEnumerable<IStepResult> GetSubSteps()
            {
                return SubSteps;
            }

            IEnumerable<FileAttachment> IStepResult.FileAttachments => FileAttachments;

            public IParameterResult[] Parameters { get; set; } = Array.Empty<IParameterResult>();
            public TestStepResult[] SubSteps { get; set; } = Array.Empty<TestStepResult>();
            public string[] Comments { get; set; } = Array.Empty<string>();
            public FileAttachment[] FileAttachments { get; set; } = Array.Empty<FileAttachment>();
        }

        public class TestStepInfo : IStepInfo
        {
            IStepNameInfo IStepInfo.Name => Name;
            public IMetadataInfo Parent { get; set; }
            public string RuntimeId { get; set; } = "11111111-1111-1111-1111-111111111111";
            public string GroupPrefix { get; set; }
            public TestStepNameInfo Name { get; set; }
            public int Number { get; set; }
            public int Total { get; set; }
            INameInfo IMetadataInfo.Name => Name;
        }

        public class TestScenarioResult : IScenarioResult
        {
            IScenarioInfo IScenarioResult.Info => Info;
            public TestScenarioInfo Info { get; set; }
            public ExecutionStatus Status { get; set; }
            public string StatusDetails { get; set; }
            public TestExecutionTime ExecutionTime { get; set; }
            public Exception ExecutionException { get; } = null;
            ExecutionTime IScenarioResult.ExecutionTime => ExecutionTime?.ToMockedType();
            public IEnumerable<IStepResult> GetSteps()
            {
                return Steps;
            }

            public TestStepResult[] Steps { get; set; }

        }

        public class TestScenarioInfo : IScenarioInfo
        {
            INameInfo IMetadataInfo.Name => Name;
            INameInfo IScenarioInfo.Name => Name;
            public string RuntimeId { get; } = "22222222-2222-2222-2222-222222222222";
            public TestNameInfo Name { get; set; }
            public IFeatureInfo Parent { get; set; }
            IEnumerable<string> IScenarioInfo.Labels => Labels;
            public string[] Labels { get; set; }
            IEnumerable<string> IScenarioInfo.Categories => Categories;
            public string[] Categories { get; set; }
        }

        public class TestFeatureResult : IFeatureResult
        {
            IFeatureInfo IFeatureResult.Info => Info;
            public TestFeatureInfo Info { get; set; }
            public TestScenarioResult[] Scenarios { get; set; }
            public IEnumerable<IScenarioResult> GetScenarios()
            {
                return Scenarios;
            }
        }

        public class TestTestRunResult : ITestRunResult
        {
            ITestRunInfo ITestRunResult.Info => Info;
            public ExecutionStatus OverallStatus { get; set; }
            public TestExecutionTime ExecutionTime { get; set; } = new();
            ExecutionTime ITestRunResult.ExecutionTime => ExecutionTime?.ToMockedType();
            public TestTestRunInfo Info { get; set; } = new();
            IReadOnlyList<IFeatureResult> ITestRunResult.Features => Features;
            public TestFeatureResult[] Features { get; set; } = Array.Empty<TestFeatureResult>();
        }

        public class TestTestRunInfo : ITestRunInfo
        {
            private static readonly AssemblyInfo[] Versions = new[] { typeof(IBddRunner).Assembly, typeof(ITestRunInfo).Assembly }
                .Select(AssemblyInfo.From).ToArray();

            public INameInfo Name => new TestNameInfo { FormattedName = TestSuite.Name, NameFormat = TestSuite.Name };
            public string RuntimeId { get; } = "33333333-5555-3333-3333-333333333333";
            public TestSuite TestSuite { get; set; } = TestSuite.Create("Random.Tests", new Version(1, 2, 3), "Foo bar");
            public IReadOnlyList<AssemblyInfo> LightBddAssemblies => Versions;
        }
        public class TestFeatureInfo : IFeatureInfo
        {
            INameInfo IMetadataInfo.Name => Name;
            INameInfo IFeatureInfo.Name => Name;
            public string RuntimeId { get; } = "33333333-3333-3333-3333-333333333333";
            public TestNameInfo Name { get; set; }
            IEnumerable<string> IFeatureInfo.Labels => Labels;
            public string[] Labels { get; set; }
            public string Description { get; set; }
            public Type FeatureType { get; set; } = typeof(TestFeatureInfo);
        }

        public class TestNameParameterInfo : INameParameterInfo
        {
            public bool IsEvaluated { get; set; }
            public ParameterVerificationStatus VerificationStatus { get; set; }
            public string FormattedValue { get; set; }
        }

        public class TestStepTypeNameInfo : IStepTypeNameInfo
        {
            public string Name { get; set; }
            public string OriginalName { get; set; }
            public override string ToString()
            {
                return Name;
            }
        }

        public class TestInlineParameterDetails : IInlineParameterDetails
        {
            public TestInlineParameterDetails(string value)
            {
                Value = value;
            }

            public TestInlineParameterDetails(string expected, string actual, ParameterVerificationStatus status, string message)
            {
                Expectation = expected;
                Value = actual;
                VerificationStatus = status;
                VerificationMessage = message;
            }

            public string VerificationMessage { get; } = "inline message";
            public ParameterVerificationStatus VerificationStatus { get; } = ParameterVerificationStatus.NotApplicable;
            public string Value { get; }
            public string Expectation { get; }
        }

        public class TestTabularParameterDetails : ITabularParameterDetails
        {
            public TestTabularParameterDetails(ParameterVerificationStatus verificationStatus)
            {
                VerificationStatus = verificationStatus;
            }

            public string VerificationMessage { get; } = "tabular message";
            public ParameterVerificationStatus VerificationStatus { get; }
            IReadOnlyList<ITabularParameterColumn> ITabularParameterDetails.Columns => Columns;
            IReadOnlyList<ITabularParameterRow> ITabularParameterDetails.Rows => Rows;

            public List<TestTabularParameterColumn> Columns { get; } = new();
            public List<TestTabularParameterRow> Rows { get; } = new();
        }

        public class TestTabularParameterRow : ITabularParameterRow
        {
            public TestTabularParameterRow(TableRowType type, ParameterVerificationStatus verificationStatus, TestValueResult[] values)
            {
                Type = type;
                VerificationStatus = verificationStatus;
                Values = values;
            }

            public TableRowType Type { get; }
            IReadOnlyList<IValueResult> ITabularParameterRow.Values => Values;
            public TestValueResult[] Values { get; }
            public string VerificationMessage { get; } = "row message";
            public ParameterVerificationStatus VerificationStatus { get; }
        }

        public class TestTabularParameterColumn : ITabularParameterColumn
        {
            public TestTabularParameterColumn(bool isKey, string name)
            {
                IsKey = isKey;
                Name = name;
            }
            public string Name { get; }
            public bool IsKey { get; }
        }

        public class TestValueResult : IValueResult
        {
            public string Value { get; set; } = "<null>";
            public string Expectation { get; set; } = "<null>";
            public string VerificationMessage { get; set; } = "value message";
            public ParameterVerificationStatus VerificationStatus { get; set; }
        }

        public class TestExecutionTime
        {
            public ExecutionTime ToMockedType()
            {
                return new ExecutionTime(Start, Duration);
            }

            public TimeSpan Duration { get; set; }
            public DateTimeOffset Start { get; set; }
        }

        public class TestParameterResult : IParameterResult
        {
            public string Name { get; }
            public IParameterDetails Details { get; }

            public TestParameterResult(string name, IParameterDetails result)
            {
                Name = name;
                Details = result;
            }
        }

        #endregion

        public static ITestRunResult CreateTestRunResults(params TestFeatureResult[] features)
        {
            var summary = ExecutionTimeSummary.Calculate(features.SelectMany(f => f.GetScenarios()).Select(s => s.ExecutionTime));
            return new TestTestRunResult
            {
                Features = features,
                ExecutionTime = new TestExecutionTime { Start = summary.Start, Duration = summary.Duration },
                OverallStatus = features.SelectMany(f => f.GetScenarios()).Max(s => s.Status) == ExecutionStatus.Failed ? ExecutionStatus.Failed : ExecutionStatus.Passed
            };
        }
    }
}