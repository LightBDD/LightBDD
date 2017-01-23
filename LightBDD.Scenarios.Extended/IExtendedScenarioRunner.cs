using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LightBDD.Scenarios.Extended
{
    public interface IExtendedScenarioRunner<TContext>
    {
        void RunScenario(params Expression<Action<TContext>>[] steps);
        Task RunScenarioAsync(params Expression<Func<TContext, Task>>[] steps);
    }
}