using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Metadata;
using LightBDD.Framework.Scenarios;

namespace LightBDD.Framework.Implementation;

//TODO: cleanup
internal class BddRunnerV2 : IBddRunner, IIntegratedScenarioBuilder<NoContext>
{
    private readonly ICoreScenarioStepsRunner _coreRunner;
    public ICoreScenarioBuilder Core { get; }

    public BddRunnerV2(ICoreScenarioStepsRunner coreRunner)
    {
        _coreRunner = coreRunner;
        Core = new Adapter(_coreRunner);
    }

    public IIntegratedScenarioBuilder<NoContext> Integrate() => this;
    public Task RunAsync() => _coreRunner.RunAsync();

    class Adapter : ICoreScenarioBuilder
    {
        private readonly ICoreScenarioStepsRunner _coreRunner;

        public Adapter(ICoreScenarioStepsRunner coreRunner)
        {
            _coreRunner = coreRunner;
        }

        public ICoreScenarioStepsRunner AddSteps(IEnumerable<StepDescriptor> steps)
        {
            return _coreRunner.AddSteps(steps);
        }

        public ICoreScenarioBuilder WithCapturedScenarioDetails()
        {
            return this;
        }

        public ICoreScenarioBuilder WithCapturedScenarioDetailsIfNotSpecified()
        {
            return this;
        }

        public ICoreScenarioBuilder WithLabels(string[] labels)
        {
            return this;
        }

        public ICoreScenarioBuilder WithCategories(string[] categories)
        {
            return this;
        }

        public ICoreScenarioBuilder WithName(string name)
        {
            return this;
        }

        public ICoreScenarioBuilder WithContext(Func<object> contextProvider, bool takeOwnership)
        {
            return this;
        }

        public ICoreScenarioBuilder WithContext(Func<IDependencyResolver, object> contextProvider, Action<ContainerConfigurator> scopeConfigurator = null)
        {
            return this;
        }

        public ICoreScenarioBuilder WithScenarioDecorators(IEnumerable<IScenarioDecorator> scenarioDecorators)
        {
            return this;
        }

        public IRunnableScenario Build()
        {
            throw new NotImplementedException();
        }

        public ICoreScenarioBuilder WithScenarioDetails(IScenarioInfo scenarioInfo)
        {
            return this;
        }

        public ICoreScenarioBuilder WithRuntimeId(string runtimeId)
        {
            return this;
        }

        ICoreScenarioBuilder ICoreScenarioBuilder.AddSteps(IEnumerable<StepDescriptor> steps)
        {
             _coreRunner.AddSteps(steps);
             return this;
        }

        public ICoreScenarioStepsRunner WithContext(ExecutionContextDescriptor contextDescriptor)
        {
            return _coreRunner.WithContext(contextDescriptor);
        }

        public Task RunAsync()
        {
            return _coreRunner.RunAsync();
        }

        public LightBddConfiguration Configuration => _coreRunner.Configuration;
    }
}