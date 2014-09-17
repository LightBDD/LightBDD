using System;
using System.Linq.Expressions;

namespace LightBDD.Execution
{
    internal class ScenarioBuilder : ICustomizedScenarioBuilder
    {
        private readonly IStepsConverter _stepsConverter;
        private readonly IScenarioExecutor _executor;
        private readonly string _scenarioName;
        private string _label;

        public ScenarioBuilder(IStepsConverter stepsConverter, IScenarioExecutor executor, string scenarioName)
        {
            if (string.IsNullOrWhiteSpace(scenarioName))
                throw new ArgumentException("Unable to create scenario without name");
            _stepsConverter = stepsConverter;
            _executor = executor;
            _scenarioName = scenarioName;
        }

        public ICustomizedScenarioBuilder WithLabel(string label)
        {
            _label = label;
            return this;
        }

        public void RunSimpleSteps<TContext>(params Action<TContext>[] steps) where TContext : new()
        {
            RunSimpleSteps(new TContext(), steps);
        }

        public void RunSimpleSteps<TContext>(TContext context, params Action<TContext>[] steps)
        {
            _executor.Execute(_scenarioName, _label, _stepsConverter.Convert(context, steps));
        }

        public void RunFormalizedSteps(params Expression<Action<StepContext>>[] steps)
        {
            RunFormalizedSteps<StepContext>(steps);
        }

        public void RunFormalizedSteps<TContext>(params Expression<Action<TContext>>[] steps) where TContext : new()
        {
            _executor.Execute(_scenarioName, _label, _stepsConverter.Convert(new TContext(), steps));
        }

        public void RunSimpleSteps(params Action[] steps)
        {
            _executor.Execute(_scenarioName, _label, _stepsConverter.Convert(steps));
        }
    }
}
