using System;
using LightBDD.Core.Execution.Results;
using LightBDD.UnitTests.Helpers;

namespace LightBDD.AcceptanceTests.Helpers.Builders
{
    internal class ScenarioBuilder
    {
        public ExecutionStatus Status { get; }
        public string[] Categories { get; private set; }

        public ScenarioBuilder(ExecutionStatus status)
        {
            Status = status;
        }

        public ScenarioBuilder WithCategories(params string[] categories)
        {
            Categories = categories;
            return this;
        }

        public Results.TestScenarioResult Build()
        {
            return Results.CreateScenarioResult("scenario", "label", DateTimeOffset.Now, TimeSpan.FromSeconds(2), Categories,
                Results.CreateStepResult(1, "step1", ExecutionStatus.Passed, DateTimeOffset.Now, TimeSpan.FromSeconds(1)),
                Results.CreateStepResult(2, "step2", Status, DateTimeOffset.Now, TimeSpan.FromSeconds(1)));
        }
    }
}