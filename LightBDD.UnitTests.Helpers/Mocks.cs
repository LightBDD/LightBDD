using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Metadata;

namespace LightBDD.UnitTests.Helpers
{
    public static class Mocks
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
            result.ExecutionTime = new ExecutionTime(executionStart, executionTime);
            return result;
        }

        public static TestStepNameInfo CreateStepName(string stepName, string stepTypeName, string nameFormat, params string[] parameters)
        {
            return new TestStepNameInfo
            {
                FormattedName = stepName,
                StepTypeName = stepTypeName,
                NameFormat = nameFormat,
                Parameters = parameters.Select(CreateStepNameParameter)
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

        public static TestScenarioResult CreateScenarioResult(string name, string label, DateTimeOffset executionStart, TimeSpan executionTime, string[] categories, params IStepResult[] steps)
        {
            return new TestScenarioResult
            {
                Info = CreateScenarioInfo(name, label, categories),
                Steps = steps,
                ExecutionTime = new ExecutionTime(executionStart, executionTime),
                Status = steps.Max(s => s.Status),
                StatusDetails = string.Join(Environment.NewLine, steps.Where(s => s.StatusDetails != null).Select(s => $"Step {s.Info.Number}: {s.StatusDetails.Trim().Replace(Environment.NewLine, Environment.NewLine + "\t")}"))
            };
        }

        private static TestScenarioInfo CreateScenarioInfo(string name, string label, string[] categories)
        {
            return new TestScenarioInfo
            {
                Name = CreateNameInfo(name),
                Labels = label != null ? new[] { label } : Enumerable.Empty<string>(),
                Categories = categories ?? new string[0]
            };
        }

        private static TestNameInfo CreateNameInfo(string name)
        {
            return new TestNameInfo { FormattedName = name };
        }

        public static TestFeatureResult CreateFeatureResult(string name, string description, string label, params IScenarioResult[] scenarios)
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
                Labels = label != null ? new[] { label } : Enumerable.Empty<string>()
            };
        }

        #region Mockable objects
        public class TestNameInfo : INameInfo
        {
            public string NameFormat { get; set; }
            public IEnumerable<INameParameterInfo> Parameters { get; set; }
            public string FormattedName { get; set; }

            public override string ToString()
            {
                return FormattedName;
            }
        }

        public class TestStepNameInfo : IStepNameInfo
        {
            public string FormattedName { get; set; }
            public string NameFormat { get; set; }
            public IEnumerable<INameParameterInfo> Parameters { get; set; }
            public string StepTypeName { get; set; }

            public override string ToString()
            {
                return FormattedName;
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
            public IEnumerable<IStepResult> GetSteps() => Steps;
            public IEnumerable<IStepResult> Steps { get; set; } = Enumerable.Empty<IStepResult>();
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
            public IEnumerable<IScenarioResult> Scenarios { get; set; } = Enumerable.Empty<IScenarioResult>();
            public IEnumerable<IScenarioResult> GetScenarios() => Scenarios;
        }

        public class TestFeatureInfo : IFeatureInfo
        {
            public INameInfo Name { get; set; }
            public IEnumerable<string> Labels { get; set; }
            public string Description { get; set; }
        }

        public class TestNameParameterInfo : INameParameterInfo
        {
            public bool IsEvaluated { get; set; }
            public string FormattedValue { get; set; }
        }
        #endregion
    }
}