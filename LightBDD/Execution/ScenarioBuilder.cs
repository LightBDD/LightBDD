using System;

namespace LightBDD.Execution
{
    internal class ScenarioBuilder : IScenarioBuilder
    {
        private readonly IStepsConverter _stepsConverter;
        private readonly IScenarioExecutor _executor;
        private readonly string _scenarioName;
        private string _label;

        public ScenarioBuilder(IStepsConverter stepsConverter, IScenarioExecutor executor, string scenarioName)
        {
            _stepsConverter = stepsConverter;
            _executor = executor;
            _scenarioName = scenarioName;
        }

        public IScenarioBuilder WithLabel(string label)
        {
            _label = label;
            return this;
        }

        public void Execute<TContext>(params Action<TContext>[] steps) where TContext : new()
        {
            Execute(new TContext(), steps);
        }

        public void Execute<TContext>(TContext context, params Action<TContext>[] steps)
        {
            _executor.Execute(_scenarioName, _label, _stepsConverter.Convert(context, steps));
        }

        public void Execute(params Action[] steps)
        {
            _executor.Execute(_scenarioName, _label, _stepsConverter.Convert(steps));
        }
    }
}
