using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;

namespace LightBDD.ScenarioHelpers
{
    public class TestScenarioBuilder
    {
        private readonly ICoreScenarioBuilder _coreBuilder;
        private Func<object>? _contextProvider;
        private bool _takeContextOwnership;
        private Func<IDependencyResolver, object>? _contextResolver;

        private TestScenarioBuilder(ICoreScenarioBuilder coreBuilder)
        {
            _coreBuilder = coreBuilder;
        }

        public static TestScenarioBuilder Current => new(ScenarioBuilderContext.Current);

        public async Task TestScenario(params StepDescriptor[] steps)
        {
            try
            {
                await WithContextIfSet()
                    .WithCapturedScenarioDetailsIfNotSpecified()
                    .AddSteps(steps)
                    .Build()
                    .ExecuteAsync();
            }
            catch (ScenarioExecutionException e)
            {
                e.GetOriginal().Throw();
            }
        }

        private ICoreScenarioBuilder WithContextIfSet()
        {
            if (_contextProvider != null)
                return _coreBuilder.WithContext(_contextProvider, _takeContextOwnership);
            if (_contextResolver != null)
                return _coreBuilder.WithContext(_contextResolver);
            return _coreBuilder;
        }
        public async Task TestScenario(params Action[] steps)
        {
            await TestScenario(steps.Select(TestStep.CreateAsync).ToArray());
        }

        public Task TestScenario(params Func<Task>[] steps)
        {
            return TestScenario(steps.Select(TestStep.Create).ToArray());
        }

        public async Task TestGroupScenario(params Func<TestCompositeStep>[] steps)
        {
            await TestScenario(steps.Select(TestStep.CreateComposite).ToArray());
        }

        public TestScenarioBuilder WithContext(Func<object> contextProvider, bool takeOwnership)
        {
            _takeContextOwnership = takeOwnership;
            _contextProvider = contextProvider;
            return this;
        }

        public TestScenarioBuilder WithContext(Func<IDependencyResolver, object> contextResolver)
        {
            _contextResolver = contextResolver;
            return this;
        }
    }
}