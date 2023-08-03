#nullable enable
using System;
using System.Threading.Tasks;
using LightBDD.Core.ExecutionContext;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Scenarios;

namespace LightBDD.Framework.Implementation;

//TODO: review
internal class BddRunnerV2 : IBddRunner, IIntegratedScenarioBuilder<NoContext>
{
    private ICoreScenarioStepsRunner? _core;
    public ICoreScenarioStepsRunner Core => _core ?? throw new InvalidOperationException("Scenario Builder not created, use Integrate() first");

    public IIntegratedScenarioBuilder<NoContext> Integrate()
    {
        _core ??= ScenarioExecutionContext.CurrentScenarioStepsRunner;
        return this;
    }

    public Task RunAsync() => Core.RunAsync();
}