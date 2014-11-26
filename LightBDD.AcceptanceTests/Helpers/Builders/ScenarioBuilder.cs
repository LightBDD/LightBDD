using System;
using LightBDD.Results;

namespace LightBDD.AcceptanceTests.Helpers.Builders
{
    internal class ScenarioBuilder
    {
        public ResultStatus Status { get; private set; }
        public string[] Categories { get; private set; }

        public ScenarioBuilder(ResultStatus status)
        {
            Status = status;
        }

        public ScenarioBuilder WithCategories(params string[] categories)
        {
            Categories = categories;
            return this;
        }

        public IScenarioResult Build()
        {
            return Mocks.CreateScenarioResult("scenario", "label", DateTimeOffset.Now, TimeSpan.FromSeconds(2), Categories,
                Mocks.CreateStepResult(1, "step1", ResultStatus.Passed, TimeSpan.FromSeconds(1)),
                Mocks.CreateStepResult(2, "step2", Status, TimeSpan.FromSeconds(1)));
        }
    }
}