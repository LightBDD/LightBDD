using System.Threading.Tasks;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Extensibility
{
    public interface ICoreBddRunner
    {
        IFeatureResult GetFeatureResult();
        IScenarioBuilder NewScenario();
        Task RunScenarioAsync(IScenarioInfo scenario);
    }
}