using System;
using System.Linq;
using LightBDD.Naming;
using LightBDD.Results;
using Rhino.Mocks;

namespace LightBDD.UnitTests.Helpers
{
    public static class Mocks
    {
        public static IStepResult CreateStepResult(int stepNumber, string stepName, ResultStatus resultStatus, DateTimeOffset executionStart, TimeSpan executionTime, string statusDetails = null)
        {
            return CreateStepResult(stepNumber, stepName, resultStatus, executionTime)
                .WithDetails(statusDetails)
                .WithExecutionStart(executionStart);
        }

        public static IStepResult CreateStepResult(int stepNumber, string stepName, ResultStatus resultStatus, TimeSpan executionTime)
        {
            return CreateStepResult(stepNumber, stepName, resultStatus)
                .WithExecutionTime(executionTime);
        }

        public static IStepResult CreateStepResult(int stepNumber, string stepName, ResultStatus resultStatus)
        {
            return CreateStepResult(stepNumber, resultStatus)
                .WithStepNameDetails(stepName, stepName)
                .WithComments();
        }

        public static IStepResult CreateStepResult(int stepNumber, ResultStatus resultStatus)
        {
            var stepResult = MockRepository.GenerateMock<IStepResult>();
            stepResult.Stub(r => r.Number).Return(stepNumber);
            stepResult.Stub(r => r.Status).Return(resultStatus);
            return stepResult;
        }

        public static IStepResult WithComments(this IStepResult result, params string[] comments)
        {
            result.Stub(r => r.Comments).Return(comments);
            return result;
        }

        public static IStepResult WithStepNameDetails(this IStepResult result, string stepName, string nameFormat, string stepTypeName = null, params string[] parameters)
        {
            result.Stub(r => r.StepName).Return(CreateStepName(stepName, stepTypeName, nameFormat, parameters));
            result.Stub(r => r.Name).Return(stepName);
            return result;
        }

        public static IStepResult WithDetails(this IStepResult result, string statusDetails)
        {
            result.Stub(r => r.StatusDetails).Return(statusDetails);
            return result;
        }

        public static IStepResult WithTimes(this IStepResult result, DateTimeOffset executionStart, TimeSpan executionTime)
        {
            return result.WithExecutionStart(executionStart)
                .WithExecutionTime(executionTime);
        }

        public static IStepResult WithExecutionStart(this IStepResult result, DateTimeOffset executionStart)
        {
            result.Stub(r => r.ExecutionStart).Return(executionStart);
            return result;
        }

        public static IStepResult WithExecutionTime(this IStepResult result, TimeSpan executionTime)
        {
            result.Stub(r => r.ExecutionTime).Return(executionTime);
            return result;
        }

        public static IStepName CreateStepName(string stepName, string stepTypeName, string nameFormat, params string[] parameters)
        {
            var name = MockRepository.GenerateMock<IStepName>();
            name.Stub(n => n.Format(Arg<IStepNameDecorator>.Is.Anything)).Return(stepName);
            name.Stub(n => n.StepTypeName).Return(stepTypeName);
            name.Stub(n => n.NameFormat).Return(nameFormat);
            name.Stub(n => n.Parameters).Return(parameters.Select(CreateStepNameParameter));
            return name;
        }

        private static IStepParameter CreateStepNameParameter(string parameter)
        {
            var param = MockRepository.GenerateMock<IStepParameter>();
            param.Stub(p => p.FormattedValue).Return(parameter);
            param.Stub(p => p.IsEvaluated).Return(true);
            return param;
        }

        public static IScenarioResult CreateScenarioResult(string name, string label, DateTimeOffset executionStart, TimeSpan executionTime, string[] categories, params IStepResult[] steps)
        {
            var result = MockRepository.GenerateMock<IScenarioResult>();
            result.Stub(r => r.Name).Return(name);
            result.Stub(r => r.Steps).Return(steps);
            result.Stub(r => r.ExecutionStart).Return(executionStart);
            result.Stub(r => r.ExecutionTime).Return(executionTime);
            result.Stub(r => r.Label).Return(label);
            result.Stub(r => r.Status).Return(steps.Max(s => s.Status));
            result.Stub(r => r.StatusDetails).Return(string.Join(Environment.NewLine, steps.Where(s => s.StatusDetails != null).Select(s => string.Format("Step {0}: {1}", s.Number, s.StatusDetails.Trim().Replace(Environment.NewLine, Environment.NewLine + "\t")))));
            result.Stub(r => r.Categories).Return(categories ?? new string[0]);
            return result;
        }

        public static IFeatureResult CreateFeatureResult(string name, string description, string label, params IScenarioResult[] scenarios)
        {
            var result = MockRepository.GenerateMock<IFeatureResult>();
            result.Stub(r => r.Name).Return(name);
            result.Stub(r => r.Description).Return(description);
            result.Stub(r => r.Label).Return(label);
            result.Stub(r => r.Scenarios).Return(scenarios);
            return result;
        }
    }
}