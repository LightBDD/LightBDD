#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Discovery;
using LightBDD.Core.ExecutionContext;
using LightBDD.Core.ExecutionContext.Implementation;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification.Events;
using LightBDD.Core.Reporting.Implementation;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Implementation;

namespace LightBDD.Core.Execution
{
    /// <summary>
    /// Execution pipeline allowing execution of LightBDD features and their scenarios.
    /// </summary>
    public class ExecutionPipeline
    {
        private readonly Assembly _testAssembly;
        private readonly Action<LightBddConfiguration>? _onConfigure;

        /// <summary>
        /// Constructor accepting the test assembly and action to customize configuration.
        /// </summary>
        /// <param name="testAssembly">Test assembly</param>
        /// <param name="onConfigure">Optional action to customize configuration</param>
        public ExecutionPipeline(Assembly testAssembly, Action<LightBddConfiguration>? onConfigure = null)
        {
            _testAssembly = testAssembly;
            _onConfigure = onConfigure;
        }

        /// <summary>
        /// Executes scenarios provided by <paramref name="scenarios"/> parameter. The execution covers entire flow of LightBDD tests, including reporting.
        /// </summary>
        /// <param name="scenarios">Scenarios to execute</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Test results</returns>
        public async Task<ITestRunResult> Execute(IReadOnlyList<ScenarioCase> scenarios, CancellationToken cancellationToken = default)
        {
            await using var ctx = new Context(Configure(), cancellationToken);
            LightBddExecutionContext.Install(ctx);
            var testRunInfo = ctx.MetadataProvider.GetTestRunInfo(_testAssembly);
            var testRunStartTime = ctx.ExecutionTimer.GetTime();
            OnBeforeTestRunStart(testRunStartTime, testRunInfo, scenarios);
            ctx.ProgressDispatcher.Notify(new TestRunStarting(testRunStartTime, testRunInfo));

            await RunGlobalSetUp(ctx);
            var results = await Task.WhenAll(scenarios.GroupBy(s => s.FeatureFixtureType).Select(fixture => ExecuteFeature(fixture, ctx)));
            await ctx.GlobalSetUp.TearDownAsync(ctx.DependencyContainer);

            var testRunEndTime = ctx.ExecutionTimer.GetTime();
            var result = new TestRunResult(testRunInfo, testRunEndTime.GetExecutionTime(testRunStartTime), results);
            ctx.ProgressDispatcher.Notify(new TestRunFinished(testRunEndTime, result));

            await GenerateReports(ctx, result);
            OnAfterTestRunFinish(testRunEndTime, result);
            LightBddExecutionContext.Clear();
            return result;
        }

        private static async Task GenerateReports(Context ctx, TestRunResult result)
        {
            await using var scope = ctx.DependencyContainer.BeginScope();
            await scope.Resolve<FeatureReportGenerator>().GenerateReports(result);
        }

        private static async Task RunGlobalSetUp(Context ctx)
        {
            try
            {
                await ctx.GlobalSetUp.SetUpAsync(ctx.DependencyContainer);
            }
            catch (Exception ex)
            {
                ctx.GlobalSetUpException = ex;
            }
        }

        /// <summary>
        /// Method executed after entire test run is finished. Can be overridden to integrate with test engine.
        /// </summary>
        /// <param name="time">Event time</param>
        /// <param name="result">Test results</param>
        protected virtual void OnAfterTestRunFinish(EventTime time, ITestRunResult result)
        {
        }

        /// <summary>
        /// Method executed before test run start. Can be overridden to integrate with test engine.
        /// </summary>
        /// <param name="time">Event time</param>
        /// <param name="testRunInfo">Test run details</param>
        /// <param name="scenarios">List of scenarios to execute</param>
        protected virtual void OnBeforeTestRunStart(EventTime time, ITestRunInfo testRunInfo, IReadOnlyList<ScenarioCase> scenarios)
        {
        }

        private async Task<IFeatureResult> ExecuteFeature(IGrouping<TypeInfo, ScenarioCase> featureScenarios, Context ctx)
        {
            var featureInfo = ctx.MetadataProvider.GetFeatureInfo(featureScenarios.Key);
            var featureStartTime = ctx.ExecutionTimer.GetTime();
            var scenarios = featureScenarios.ToArray();
            OnBeforeFeatureStart(featureStartTime, featureInfo, scenarios);
            ctx.ProgressDispatcher.Notify(new FeatureStarting(featureStartTime, featureInfo));

            var results = await Task.WhenAll(featureScenarios.SelectMany(s => ExpandParameterizedScenarios(s, ctx))
                .GroupBy(s => s.ScenarioMethod)
                .Select(s => ExecuteScenarioGroup(featureInfo, s, ctx)));

            var featureEndTime = ctx.ExecutionTimer.GetTime();
            var result = new FeatureResultV2(featureInfo, results.SelectMany(r => r).ToArray());
            ctx.ProgressDispatcher.Notify(new FeatureFinished(featureEndTime, result));
            OnAfterFeatureFinished(featureEndTime, result);
            return result;
        }

        /// <summary>
        /// Method executed after all scenarios for given feature are finished. Can be overridden to integrate with test engine.
        /// </summary>
        /// <param name="time">Event time</param>
        /// <param name="result">Feature result</param>
        protected virtual void OnAfterFeatureFinished(EventTime time, IFeatureResult result)
        {
        }

        /// <summary>
        /// Method executed before feature scenarios execution. Can be overridden to integrate with test engine.
        /// </summary>
        /// <param name="time">Event time</param>
        /// <param name="featureInfo">Feature details</param>
        /// <param name="scenarios">Scenarios to run as part of this feature</param>
        protected virtual void OnBeforeFeatureStart(EventTime time, IFeatureInfo featureInfo, ScenarioCase[] scenarios)
        {
        }

        private async Task<IScenarioResult[]> ExecuteScenarioGroup(IFeatureInfo featureInfo, IGrouping<MethodInfo, ScenarioCase> scenarioCases, Context ctx)
        {
            var scenarios = scenarioCases.ToArray();
            OnBeforeScenarioGroup(ctx.ExecutionTimer.GetTime(), scenarioCases.Key, scenarios);
            var results = await Task.WhenAll(scenarios.Select(s => ctx.ExecutionScheduler.Schedule(() => ExecuteScenario(featureInfo, s, ctx))));
            OnAfterScenarioGroup(ctx.ExecutionTimer.GetTime(), results);
            return results;
        }

        /// <summary>
        /// Method executed after all scenarios are executed within the group (fixture type-scenario method pair). Can be overridden to integrate with test engine.
        /// </summary>
        /// <param name="time">Event time</param>
        /// <param name="results">List of scenario results</param>
        protected virtual void OnAfterScenarioGroup(EventTime time, IReadOnlyList<IScenarioResult> results)
        {
        }

        /// <summary>
        /// Method executed before scenario group execution (fixture type-scenario method pair). Can be overridden to integrate with test engine.
        /// </summary>
        /// <param name="time">Event time</param>
        /// <param name="scenarioMethod">Scenario method</param>
        /// <param name="scenarios">Scenarios to execute</param>
        protected virtual void OnBeforeScenarioGroup(EventTime time, MethodInfo scenarioMethod, IReadOnlyList<ScenarioCase> scenarios)
        {
        }

        //TODO: simplify
        private async Task<IScenarioResult> ExecuteScenario(IFeatureInfo featureInfo, ScenarioCase scenario, Context ctx)
        {
            var runnableScenario = CreateRunnableScenario(featureInfo, scenario, ctx);
            var scenarioInfo = runnableScenario.Result.Info;
            var scenarioResult = runnableScenario.Result;
            try
            {
                OnBeforeScenario(ctx.ExecutionTimer.GetTime(), scenarioInfo, scenario);

                if (ctx.GlobalSetUpException != null)
                    scenarioResult = ScenarioResult.CreateFailed(scenarioInfo, ctx.GlobalSetUpException);
                else if (ctx.CancellationToken.IsCancellationRequested)
                    scenarioResult = ScenarioResult.CreateIgnored(scenarioInfo, "Execution aborted");
                else
                    scenarioResult = await runnableScenario.RunAsync();
            }
            catch (Exception ex)
            {
                scenarioResult = ScenarioResult.CreateFailed(scenarioInfo, ex);
            }
            finally
            {
                OnAfterScenario(ctx.ExecutionTimer.GetTime(), scenarioResult);
            }

            return scenarioResult;
        }

        /// <summary>
        /// Method executed before feature scenario execution (and before construction of any dependencies related to scenario execution). Can be overridden to integrate with test engine.
        /// </summary>
        /// <param name="time">Event time</param>
        /// <param name="scenarioInfo">Scenario details</param>
        /// <param name="scenarioCase">Scenario case</param>
        protected virtual void OnBeforeScenario(EventTime time, IScenarioInfo scenarioInfo, ScenarioCase scenarioCase)
        {
        }

        /// <summary>
        /// Method executed after scenario execution and after scenario dependencies disposal. Can be overridden to integrate with test engine.
        /// </summary>
        /// <param name="time">Event time</param>
        /// <param name="result">Scenario result</param>
        protected virtual void OnAfterScenario(EventTime time, IScenarioResult result)
        {
        }

        private static IRunnableScenarioV2 CreateRunnableScenario(IFeatureInfo featureInfo, ScenarioCase scenario, Context ctx)
        {
            var descriptor = new ScenarioDescriptor(scenario.ScenarioMethod, scenario.ScenarioArguments);
            return ctx.ScenarioFactory.CreateFor(featureInfo)
                .WithName(ctx.MetadataProvider.GetScenarioName(descriptor))
                .WithCategories(ctx.MetadataProvider.GetScenarioCategories(scenario.ScenarioMethod))
                .WithLabels(ctx.MetadataProvider.GetScenarioLabels(scenario.ScenarioMethod))
                .WithRuntimeId(scenario.RuntimeId ?? Guid.NewGuid().ToString())
                .WithScenarioDecorators(ctx.MetadataProvider.GetScenarioDecorators(descriptor))
                .WithScenarioEntryMethod((fixture, runner) => InvokeScenarioEntryMethod(scenario.ScenarioMethod, fixture, scenario.ScenarioArguments, runner))
                .Build();
        }

        //TODO: improve?
        private static async Task InvokeScenarioEntryMethod(MethodInfo methodInfo, object fixture, object[] arguments, ICoreScenarioStepsRunner coreScenarioStepsRunner)
        {
            object? result;
            try
            {
                ScenarioExecutionContext.Current.Get<CurrentScenarioProperty>().StepsRunner = coreScenarioStepsRunner;
                result = methodInfo.Invoke(fixture, arguments);
            }
            catch (TargetInvocationException e) when (e.InnerException != null)
            {
                ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                throw;
            }
            finally
            {
                ScenarioExecutionContext.Current.Get<CurrentScenarioProperty>().StepsRunner = null;
            }

            if (result is Task taskResult)
                await taskResult;
        }

        private IEnumerable<ScenarioCase> ExpandParameterizedScenarios(ScenarioCase scenarioCase, Context ctx)
        {
            if (!scenarioCase.RequireArgumentResolutionAtRuntime)
            {
                yield return scenarioCase;
                yield break;
            }
            throw new NotImplementedException();
        }

        private LightBddConfiguration Configure()
        {
            var cfg = new LightBddConfiguration();
            _onConfigure?.Invoke(cfg);
            return cfg;
        }

        //TODO: review
        private class Context : EngineContext, IAsyncDisposable
        {
            public Context(LightBddConfiguration configuration, CancellationToken cancellationToken)
            : base(configuration)
            {
                CancellationToken = cancellationToken;
                ScenarioFactory = new RunnableScenarioFactory(this);
            }

            public readonly RunnableScenarioFactory ScenarioFactory;
            public readonly CancellationToken CancellationToken;
            public Exception? GlobalSetUpException;

            public ValueTask DisposeAsync() => DependencyContainer.DisposeAsync();
        }
    }
}
