using System;
using System.Linq.Expressions;

namespace LightBDD.Execution
{
    internal class ScenarioBuilder : ICustomizedScenarioBuilder
    {
        private readonly IStepsConverter _stepsConverter;
        private readonly IScenarioExecutor _executor;
        private readonly Scenario _scenario = new Scenario();

        public ScenarioBuilder(IStepsConverter stepsConverter, IScenarioExecutor executor, string scenarioName)
        {
            if (string.IsNullOrWhiteSpace(scenarioName))
                throw new ArgumentException("Unable to create scenario without name");
            _stepsConverter = stepsConverter;
            _executor = executor;
            _scenario.Name = scenarioName;
        }

        public ICustomizedScenarioBuilder WithLabel(string label)
        {
            _scenario.Label = label;
            return this;
        }

        public void RunFormalizedSteps(params Expression<Action<StepType>>[] steps)
        {
            _executor.Execute(_scenario, _stepsConverter.Convert(steps));
        }

        public IScenarioBuilder<TContext> WithContext<TContext>() where TContext : new()
        {
            return WithContext(new TContext());
        }

        public IScenarioBuilder<TContext> WithContext<TContext>(TContext instance)
        {
            return new ScenarioBuilder<TContext>(_stepsConverter, _executor, _scenario, instance);
        }

        public void RunSimpleSteps(params Action[] steps)
        {
            _executor.Execute(_scenario, _stepsConverter.Convert(steps));
        }
    }

    internal class ScenarioBuilder<TContext> : IScenarioBuilder<TContext>
    {
        private readonly IStepsConverter _stepsConverter;
        private readonly IScenarioExecutor _executor;
        private readonly Scenario _scenario;
        private readonly TContext _context;

        public ScenarioBuilder(IStepsConverter stepsConverter, IScenarioExecutor executor, Scenario scenario, TContext context)
        {
            _stepsConverter = stepsConverter;
            _executor = executor;
            _scenario = scenario;
            _context = context;
        }

        public void RunFormalizedSteps(params Expression<Action<StepType, TContext>>[] steps)
        {
            _executor.Execute(_scenario, _stepsConverter.Convert(_context, steps));
        }

        public void RunSimpleSteps(params Action<TContext>[] steps)
        {
            _executor.Execute(_scenario, _stepsConverter.Convert(_context, steps));
        }
    }
}
