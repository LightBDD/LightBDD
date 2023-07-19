using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;

namespace LightBDD.XUnit2.Implementation;

internal class BddRunnerAdapter : IBddRunner, IIntegratedScenarioBuilder<NoContext>
{
    public static readonly BddRunnerAdapter Instance = new();
    private BddRunnerAdapter() { }

    public IIntegratedScenarioBuilder<NoContext> Integrate() => this;
    public async Task RunAsync()
    {
        try
        {
            await ScenarioBuilderContext.Current.Build().ExecuteAsync();
        }
        catch (ScenarioExecutionException e)
        {
            e.GetOriginal().Throw();
        }
    }

    public ICoreScenarioBuilder Core => ScenarioBuilderContext.Current;
}