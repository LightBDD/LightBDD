using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Formatting.NameDecorators;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results;

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
            return new TestScenarioResult
            {
                Info = CreateScenarioInfo(name, label, categories),
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
                Labels = label != null ? new[] { label } : new string[0],
                Categories = categories ?? new string[0]
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
            return new TestFeatureResult
            {
                Info = CreateFeatureInfo(name, description, label),
                Scenarios = scenarios
            };
        }

        private static TestFeatureInfo CreateFeatureInfo(string name, string description, string label)
        {
            return new TestFeatureInfo
            {
                Name = CreateNameInfo(name),
                Description = description,
                Labels = label != null ? new[] { label } : new string[0]
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

            public string Format(IStepNameDecorator stepNameDecorator)
            {
                return FormattedName;
            }

            public string Format(INameDecorator decorator)
            {
                return FormattedName;
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
            IEnumerable<IStepResult> IStepResult.SubSteps => SubSteps;
            public IStepResult[] SubSteps { get; set; }
            public string[] Comments { get; set; }
        }

        public class TestStepInfo : IStepInfo
        {
            IStepNameInfo IStepInfo.Name => Name;
            public string GroupPrefix { get; set; }
            public TestStepNameInfo Name { get; set; }
            public int Number { get; set; }
            public int Total { get; set; }
        }

        public class TestScenarioResult : IScenarioResult
        {
            IScenarioInfo IScenarioResult.Info => Info;
            public TestScenarioInfo Info { get; set; }
            public ExecutionStatus Status { get; set; }
            public string StatusDetails { get; set; }
            public TestExecutionTime ExecutionTime { get; set; }
            ExecutionTime IScenarioResult.ExecutionTime => ExecutionTime?.ToMockedType();
            public IEnumerable<IStepResult> GetSteps() => Steps;
            public TestStepResult[] Steps { get; set; }

        }

        public class TestScenarioInfo : IScenarioInfo
        {
            INameInfo IScenarioInfo.Name => Name;
            public TestNameInfo Name { get; set; }
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
            public IEnumerable<IScenarioResult> GetScenarios() => Scenarios;
        }

        public class TestFeatureInfo : IFeatureInfo
        {
            INameInfo IFeatureInfo.Name => Name;
            public TestNameInfo Name { get; set; }
            IEnumerable<string> IFeatureInfo.Labels => Labels;
            public string[] Labels { get; set; }
            public string Description { get; set; }
        }

        public class TestNameParameterInfo : INameParameterInfo
        {
            public bool IsEvaluated { get; set; }
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

        public class TestExecutionTime
        {
            public ExecutionTime ToMockedType()
            {
                return new ExecutionTime(Start, Duration);
            }

            public TimeSpan Duration { get; set; }
            public DateTimeOffset Start { get; set; }
        }
        #endregion
    }
}