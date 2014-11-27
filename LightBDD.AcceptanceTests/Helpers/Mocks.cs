using System;
using System.Linq;
using LightBDD.Naming;
using LightBDD.Results;
using Rhino.Mocks;

namespace LightBDD.AcceptanceTests.Helpers
{
    public static class Mocks
    {
        public static IStepResult CreateStepResult(int stepNumber, string stepName, ResultStatus resultStatus, DateTimeOffset executionStart, TimeSpan executionTime, string statusDetails = null)
        {
            var stepResult = CreateStepResult(stepNumber, stepName, resultStatus, executionTime);
            stepResult.Stub(r => r.ExecutionStart).Return(executionStart);
            stepResult.Stub(r => r.StatusDetails).Return(statusDetails);
            return stepResult;
        }

        public static IStepResult CreateStepResult(int stepNumber, string stepName, ResultStatus resultStatus, TimeSpan executionTime)
        {
            var stepResult = CreateStepResult(stepNumber, stepName, resultStatus);
            stepResult.Stub(r => r.ExecutionTime).Return(executionTime);
            return stepResult;
        }

        public static IStepResult CreateStepResult(int stepNumber, string stepName, ResultStatus resultStatus)
        {
            var stepResult = MockRepository.GenerateMock<IStepResult>();
            stepResult.Stub(r => r.Name).Return(stepName);
            stepResult.Stub(r => r.Number).Return(stepNumber);
            stepResult.Stub(r => r.Status).Return(resultStatus);
            stepResult.Stub(r => r.StepName).Return(CreateStepName(stepName));
            return stepResult;
        }

        public static IStepName CreateStepName(string stepName)
        {
            var name = MockRepository.GenerateMock<IStepName>();
            name.Stub(n => n.Format(Arg<IStepNameDecorator>.Is.Anything)).Return(stepName);
            return name;
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