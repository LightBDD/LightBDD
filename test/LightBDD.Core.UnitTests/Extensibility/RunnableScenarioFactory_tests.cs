using System;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.UnitTests.Helpers;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Extensibility
{
    [TestFixture]
    public class RunnableScenarioFactory_tests
    {
        [Test]
        public void Build_should_set_configured_feature_info()
        {
            var info = Fake.Object<TestResults.TestFeatureInfo>();
            var scenario = TestableScenarioFactory.Default.CreateBuilder(info).Build();
            scenario.Result.Info.Parent.ShouldBeSameAs(info);
        }

        [Test]
        public void Build_should_set_configured_scenario_name()
        {
            //TODO: review option to construct name without need of using custom INameInfo implementations.
            var name = Fake.Object<TestResults.TestNameInfo>();
            var scenario = TestableScenarioFactory.Default.CreateBuilder()
                .WithName(name)
                .Build();
            scenario.Result.Info.Name.ShouldBe(name);
        }

        [Test]
        public void Build_should_set_configured_labels()
        {
            var labels = Fake.StringArray();
            var scenario = TestableScenarioFactory.Default.CreateBuilder()
                .WithLabels(labels)
                .Build();
            scenario.Result.Info.Labels.ShouldBe(labels);
        }

        [Test]
        public void Build_should_set_configured_categories()
        {
            var categories = Fake.StringArray();
            var scenario = TestableScenarioFactory.Default.CreateBuilder()
                .WithCategories(categories)
                .Build();
            scenario.Result.Info.Categories.ShouldBe(categories);
        }

        [Test]
        public void Build_should_set_configured_runtimeId()
        {
            var runtimeId = Fake.String();
            var scenario = TestableScenarioFactory.Default.CreateBuilder()
                .WithRuntimeId(runtimeId)
                .Build();
            scenario.Result.Info.RuntimeId.ShouldBe(runtimeId);
        }

        [Test]
        public void Build_should_set_default_values_when_not_customized()
        {
            var scenario = TestableScenarioFactory.Default.CreateBuilder()
                .Build();
            scenario.Result.Info.Categories.ShouldBeEmpty();
            scenario.Result.Info.Labels.ShouldBeEmpty();
            Guid.TryParse(scenario.Result.Info.RuntimeId, out _).ShouldBeTrue();
            scenario.Result.Info.Name.ToString().ShouldBe("<not specified>");
        }

        [Test]
        public void Build_should_create_scenario_in_NotRun_status()
        {
            var scenario = TestableScenarioFactory.Default.CreateBuilder()
                .Build();
            scenario.Result.Status.ShouldBe(ExecutionStatus.NotRun);
            scenario.Result.ExecutionException.ShouldBeNull();
            scenario.Result.ExecutionTime.ShouldBe(ExecutionTime.None);
            scenario.Result.StatusDetails.ShouldBeNull();
            scenario.Result.GetSteps().ShouldBeEmpty();
        }

        [Test]
        public async Task Build_should_set_configured_entry_method()
        {
            var executed = false;
            Task EntryMethod(object _, ICoreScenarioStepsRunner __)
            {
                executed = true;
                return Task.CompletedTask;
            }

            var scenario = TestableScenarioFactory.Default.CreateBuilder()
                .WithScenarioEntryMethod(EntryMethod)
                .Build();
            await scenario.RunAsync();
            executed.ShouldBeTrue();
        }

        [Test]
        public async Task Build_should_set_scenario_decorators()
        {
            var d1 = new FakeDecorator();
            var d2 = new FakeDecorator();
            var scenario = TestableScenarioFactory.Default.CreateBuilder()
                .WithScenarioDecorators(new[] { d1, d2 })
                .Build();

            await scenario.RunAsync();

            d1.Executed.ShouldBeTrue();
            d2.Executed.ShouldBeTrue();
        }

        [Test]
        public void Build_should_create_runnable_scenario()
        {
            var scenario = TestableScenarioFactory.Default.CreateBuilder()
                .Build();
            Assert.DoesNotThrowAsync(scenario.RunAsync);
        }

        class FakeDecorator : IScenarioDecorator
        {
            public Task ExecuteAsync(IScenario scenario, Func<Task> scenarioInvocation)
            {
                Executed = true;
                return scenarioInvocation.Invoke();
            }

            public bool Executed { get; private set; }
        }
    }
}
