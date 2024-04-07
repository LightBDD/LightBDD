using System;
using System.Collections.Generic;
using LightBDD.Core.Results;
using LightBDD.UnitTests.Helpers;

namespace LightBDD.AcceptanceTests.Helpers.Builders
{
    internal class ScenarioBuilder
    {
        private readonly List<TestResults.TestStepResult> _steps = new();
        public ExecutionStatus Status { get; }
        public string[] Categories { get; private set; }
        public string Name { get; private set; } = "scenario";
        public string Description { get; private set; } = null;

        public ScenarioBuilder(ExecutionStatus status)
        {
            Status = status;
        }

        public ScenarioBuilder WithCategories(params string[] categories)
        {
            Categories = categories;
            return this;
        }

        public ScenarioBuilder WithName(string name)
        {
            Name = name;
            return this;
        }

        public ScenarioBuilder WithDescription(string description)
        {
            Description = description;
            return this;
        }

        public ScenarioBuilder WithSampleSteps()
        {
            _steps.Add(TestResults.CreateStepResult(1, "step1", ExecutionStatus.Passed, DateTimeOffset.Now, TimeSpan.FromSeconds(1)).WithRuntimeId(Guid.NewGuid()).WithSubSteps(
                TestResults.CreateStepResult(1, "sub-step1", ExecutionStatus.Passed, DateTimeOffset.Now, TimeSpan.FromMilliseconds(500))).WithRuntimeId(Guid.NewGuid()));
            _steps.Add(TestResults.CreateStepResult(2, "step2", Status, DateTimeOffset.Now, TimeSpan.FromSeconds(1)).WithRuntimeId(Guid.NewGuid()));

            return this;
        }

        public TestResults.TestScenarioResult Build()
        {
            return TestResults.CreateScenarioResult(Name, "label", Description, DateTimeOffset.Now, TimeSpan.FromSeconds(2), Categories, _steps.ToArray());
        }

        public TestResults.TestStepResult AddStep(string name, ExecutionStatus status)
        {
            var step = TestResults.CreateStepResult(_steps.Count + 1, name, status, DateTimeOffset.Now,
                TimeSpan.FromSeconds(1));
            _steps.Add(step);
            return step;
        }
    }
}