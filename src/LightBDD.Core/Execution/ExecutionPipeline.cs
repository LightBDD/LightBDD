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
            var results = await RunScenarios(scenarios, ctx);
            await ctx.GlobalSetUp.TearDownAsync(ctx.DependencyContainer);

            var testRunEndTime = ctx.ExecutionTimer.GetTime();
            var result = new TestRunResult(testRunInfo, testRunEndTime.GetExecutionTime(testRunStartTime), results);
            ctx.ProgressDispatcher.Notify(new TestRunFinished(testRunEndTime, result));

            await GenerateReports(ctx, result);
            OnAfterTestRunFinish(testRunEndTime, result);
            LightBddExecutionContext.Clear();
            return result;
        }

        private async Task<IFeatureResult[]> RunScenarios(IReadOnlyList<ScenarioCase> scenarios, Context ctx)
        {
            var runnableFeatures = PrepareFeatures(scenarios, ctx);

            await Task.WhenAll(runnableFeatures.SelectMany(f => f.GetRunnableCases())
                .OrderBy(c => c.Priority)
                .ThenBy(c => c.Scenario.FeatureFixtureType.Name)
                .ThenBy(c => c.Scenario.ScenarioMethod.Name)
                .Select(c => ctx.ExecutionScheduler.Schedule(c.Execute)));

            return runnableFeatures.Select(r => r.Result!).ToArray();
        }

        private IReadOnlyList<RunnableFeature> PrepareFeatures(IReadOnlyList<ScenarioCase> scenarios, Context ctx)
        {
            var features = new List<RunnableFeature>();

            foreach (var featureGrouping in scenarios.GroupBy(s => s.FeatureFixtureType))
            {
                var feature = new RunnableFeature(ctx.MetadataProvider.GetFeatureInfo(featureGrouping.Key), ctx);
                feature.AddCallbacks(OnBeforeFeatureStart, OnAfterFeatureFinished);
                features.Add(feature);

                foreach (var scenarioGrouping in featureGrouping.GroupBy(s => s.ScenarioMethod))
                {
                    var scenarioGroup = new RunnableScenarioGroup(ctx);
                    feature.Add(scenarioGroup);
                    scenarioGroup.AddCallbacks(OnBeforeScenarioGroupStart, OnAfterScenarioGroupFinished);

                    foreach (var scenarioCase in scenarioGrouping.SelectMany(g => ExpandParameterizedScenarios(g, ctx)))
                    {
                        var scenario = new RunnableScenarioCase(feature.Info, scenarioCase, ctx);
                        scenarioGroup.Add(scenario);
                        scenario.AddCallbacks(OnBeforeScenarioStart, OnAfterScenarioFinished);
                    }
                }
            }

            return features;
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
        protected virtual void OnBeforeFeatureStart(EventTime time, IFeatureInfo featureInfo, IReadOnlyList<ScenarioCase> scenarios)
        {
        }

        /// <summary>
        /// Method executed after all scenarios are executed within the group (fixture type-scenario method pair). Can be overridden to integrate with test engine.
        /// </summary>
        /// <param name="time">Event time</param>
        /// <param name="runtimeInfo">Scenario group info</param>
        /// <param name="results">List of scenario results</param>
        protected virtual void OnAfterScenarioGroupFinished(EventTime time, IRuntimeObjectInfo runtimeInfo, IReadOnlyList<IScenarioResult> results)
        {
        }

        /// <summary>
        /// Method executed before scenario group execution (fixture type-scenario method pair). Can be overridden to integrate with test engine.
        /// </summary>
        /// <param name="time">Event time</param>
        /// <param name="runtimeInfo">Scenario group info</param>
        /// <param name="scenarios">Scenarios to execute</param>
        protected virtual void OnBeforeScenarioGroupStart(EventTime time, IRuntimeObjectInfo runtimeInfo, IReadOnlyList<ScenarioCase> scenarios)
        {
        }

        /// <summary>
        /// Method executed before feature scenario execution (and before construction of any dependencies related to scenario execution). Can be overridden to integrate with test engine.
        /// </summary>
        /// <param name="time">Event time</param>
        /// <param name="scenarioInfo">Scenario details</param>
        /// <param name="scenarioCase">Scenario case</param>
        protected virtual void OnBeforeScenarioStart(EventTime time, IScenarioInfo scenarioInfo, ScenarioCase scenarioCase)
        {
        }

        /// <summary>
        /// Method executed after scenario execution and after scenario dependencies disposal. Can be overridden to integrate with test engine.
        /// </summary>
        /// <param name="time">Event time</param>
        /// <param name="result">Scenario result</param>
        protected virtual void OnAfterScenarioFinished(EventTime time, IScenarioResult result)
        {
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

        class RunnableFeature
        {
            private volatile bool _started;
            private volatile int _finished;
            private readonly Context _ctx;
            private readonly List<RunnableScenarioGroup> _scenarioGroups = new();
            public IFeatureInfo Info { get; set; }

            private Action<EventTime, IFeatureInfo, IReadOnlyList<ScenarioCase>>? _onBeforeStart;
            private Action<EventTime, IFeatureResult>? _onAfterFinished;


            public RunnableFeature(IFeatureInfo featureInfo, Context ctx)
            {
                _ctx = ctx;
                Info = featureInfo;
            }

            public void AddCallbacks(Action<EventTime, IFeatureInfo, IReadOnlyList<ScenarioCase>> onBeforeStart, Action<EventTime, IFeatureResult> onAfterFinished)
            {
                _onBeforeStart += onBeforeStart;
                _onAfterFinished = onAfterFinished + _onAfterFinished;
            }

            private void HandleGroupStart(EventTime e, IRuntimeObjectInfo runtimeObjectInfo, IReadOnlyList<ScenarioCase> arg3)
            {
                if (_started)
                    return;
                lock (this)
                {
                    if (_started)
                        return;
                    try
                    {
                        //TODO: review
                        _onBeforeStart?.Invoke(_ctx.ExecutionTimer.GetTime(), Info, _scenarioGroups.SelectMany(s => s.Scenarios).Select(s => s.Scenario).ToArray());
                        _ctx.ProgressDispatcher.Notify(new FeatureStarting(e, Info));
                    }
                    finally
                    {
                        _started = true;
                    }
                }
            }

            private void HandleGroupFinished(EventTime e, IRuntimeObjectInfo runtimeObjectInfo, IReadOnlyList<IScenarioResult> arg3)
            {
                if (Interlocked.Increment(ref _finished) == _scenarioGroups.Count)
                {
                    Result = new FeatureResultV2(Info, _scenarioGroups.SelectMany(s => s.Scenarios).Select(s => s.Result!).ToArray());
                    _ctx.ProgressDispatcher.Notify(new FeatureFinished(e, Result));
                    _onAfterFinished?.Invoke(_ctx.ExecutionTimer.GetTime(), Result);
                }
            }

            public IFeatureResult? Result { get; private set; }

            public IEnumerable<RunnableScenarioCase> GetRunnableCases() => _scenarioGroups.SelectMany(g => g.Scenarios);

            public void Add(RunnableScenarioGroup scenarioGroup)
            {
                scenarioGroup.AddCallbacks(HandleGroupStart, HandleGroupFinished);
                _scenarioGroups.Add(scenarioGroup);
            }
        }

        class RunnableScenarioGroup : IRuntimeObjectInfo
        {
            private readonly Context _ctx;
            private readonly List<RunnableScenarioCase> _scenarios = new();
            private Action<EventTime, IRuntimeObjectInfo, IReadOnlyList<ScenarioCase>>? _onBeforeStart;
            private Action<EventTime, IRuntimeObjectInfo, IReadOnlyList<IScenarioResult>>? _onAfterFinished;
            private volatile bool _started;
            private volatile int _finished;
            public string RuntimeId { get; } = Guid.NewGuid().ToString();
            public IReadOnlyList<RunnableScenarioCase> Scenarios => _scenarios;


            public RunnableScenarioGroup(Context ctx)
            {
                _ctx = ctx;
            }

            public void AddCallbacks(Action<EventTime, IRuntimeObjectInfo, IReadOnlyList<ScenarioCase>> onBeforeStart, Action<EventTime, IRuntimeObjectInfo, IReadOnlyList<IScenarioResult>> onAfterFinished)
            {
                _onBeforeStart += onBeforeStart;
                _onAfterFinished = onAfterFinished + _onAfterFinished;
            }

            private void HandleScenarioStart(EventTime e, IScenarioInfo s, ScenarioCase c)
            {
                if (_started)
                    return;
                lock (this)
                {
                    if (_started)
                        return;
                    try
                    {
                        //TODO: review
                        _onBeforeStart?.Invoke(_ctx.ExecutionTimer.GetTime(), this, Scenarios.Select(x => x.Scenario).ToArray());
                    }
                    finally
                    {
                        _started = true;
                    }
                }
            }

            private void HandleScenarioFinished(EventTime e, IScenarioResult r)
            {
                if (Interlocked.Increment(ref _finished) == _scenarios.Count)
                    _onAfterFinished?.Invoke(_ctx.ExecutionTimer.GetTime(), this, _scenarios.Select(s => s.Result!).ToArray());
            }

            public void Add(RunnableScenarioCase scenario)
            {
                scenario.AddCallbacks(HandleScenarioStart, HandleScenarioFinished);
                _scenarios.Add(scenario);
            }
        }

        class RunnableScenarioCase
        {
            private readonly IFeatureInfo _feature;
            private readonly Context _ctx;

            public RunnableScenarioCase(IFeatureInfo feature, ScenarioCase scenario, Context ctx)
            {
                _feature = feature;
                Scenario = scenario;
                _ctx = ctx;
            }

            //TODO: implement
            public int Priority { get; } = 10;
            public ScenarioCase Scenario { get; }
            public IScenarioResult? Result { get; private set; }

            private Action<EventTime, IScenarioInfo, ScenarioCase>? _onBeforeStart;
            private Action<EventTime, IScenarioResult>? _onAfterFinished;

            public void AddCallbacks(Action<EventTime, IScenarioInfo, ScenarioCase> onBeforeStart, Action<EventTime, IScenarioResult> onAfterFinished)
            {
                _onBeforeStart += onBeforeStart;
                _onAfterFinished = onAfterFinished + _onAfterFinished;
            }

            public async Task<IScenarioResult> Execute()
            {
                var runnableScenario = CreateRunnableScenario();
                var scenarioInfo = runnableScenario.Result.Info;
                Result = runnableScenario.Result;
                try
                {
                    _onBeforeStart?.Invoke(_ctx.ExecutionTimer.GetTime(), scenarioInfo, Scenario);

                    if (_ctx.GlobalSetUpException != null)
                        Result = ScenarioResult.CreateFailed(scenarioInfo, _ctx.GlobalSetUpException);
                    else if (_ctx.CancellationToken.IsCancellationRequested)
                        Result = ScenarioResult.CreateIgnored(scenarioInfo, "Execution aborted");
                    else
                        Result = await runnableScenario.RunAsync();
                }
                catch (Exception ex)
                {
                    Result = ScenarioResult.CreateFailed(scenarioInfo, ex);
                }
                finally
                {
                    _onAfterFinished?.Invoke(_ctx.ExecutionTimer.GetTime(), Result);
                }

                return Result;
            }

            private IRunnableScenarioV2 CreateRunnableScenario()
            {
                var descriptor = new ScenarioDescriptor(Scenario.ScenarioMethod, Scenario.ScenarioArguments);
                return _ctx.ScenarioFactory.CreateFor(_feature)
                    .WithName(_ctx.MetadataProvider.GetScenarioName(descriptor))
                    .WithCategories(_ctx.MetadataProvider.GetScenarioCategories(Scenario.ScenarioMethod))
                    .WithLabels(_ctx.MetadataProvider.GetScenarioLabels(Scenario.ScenarioMethod))
                    .WithRuntimeId(Scenario.RuntimeId ?? Guid.NewGuid().ToString())
                    .WithScenarioDecorators(_ctx.MetadataProvider.GetScenarioDecorators(descriptor))
                    .WithScenarioEntryMethod((fixture, runner) => InvokeScenarioEntryMethod(Scenario.ScenarioMethod, fixture, Scenario.ScenarioArguments, runner))
                    .Build();
            }
        }
    }
}
