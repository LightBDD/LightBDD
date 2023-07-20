#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Discovery;
using LightBDD.Core.Execution.Implementation;
using LightBDD.Core.Execution.Implementation.GlobalSetUp;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Extensibility.Implementation;
using LightBDD.Core.Formatting;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;
using LightBDD.Core.Notification;
using LightBDD.Core.Notification.Events;
using LightBDD.Core.Reporting;
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
            using var ctx = new Context(_testAssembly, Configure(), cancellationToken);
            var testRunInfo = ctx.MetadataProvider.GetTestRunInfo();
            var testRunStartTime = ctx.Timer.GetTime();
            OnBeforeTestRunStart(testRunStartTime, testRunInfo, scenarios);
            ctx.ProgressNotifier.Notify(new TestRunStarting(testRunStartTime, testRunInfo));

            await RunGlobalSetUp(ctx);
            var results = await Task.WhenAll(scenarios.GroupBy(s => s.FeatureFixtureType).Select(fixture => ExecuteFeature(fixture, ctx)));
            await ctx.GlobalSetUp.TearDownAsync(ctx.DependencyContainer);

            var testRunEndTime = ctx.Timer.GetTime();
            var result = new TestRunResult(testRunInfo, testRunEndTime.GetExecutionTime(testRunStartTime), results);
            ctx.ProgressNotifier.Notify(new TestRunFinished(testRunEndTime, result));

            await new FeatureReportGenerator(ctx.Configuration).GenerateReports(result);
            OnAfterTestRunFinish(testRunEndTime, result);
            return result;
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
            var featureStartTime = ctx.Timer.GetTime();
            var scenarios = featureScenarios.ToArray();
            OnBeforeFeatureStart(featureStartTime, featureInfo, scenarios);
            ctx.ProgressNotifier.Notify(new FeatureStarting(featureStartTime, featureInfo));

            var results = await Task.WhenAll(featureScenarios.SelectMany(s => ExpandParameterizedScenarios(s, ctx))
                .GroupBy(s => s.ScenarioMethod)
                .Select(s => ExecuteScenarioGroup(featureInfo, s, ctx)));

            var featureEndTime = ctx.Timer.GetTime();
            var result = new FeatureResultV2(featureInfo, results.SelectMany(r => r).ToArray());
            ctx.ProgressNotifier.Notify(new FeatureFinished(featureEndTime, result));
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
            var startTime = ctx.Timer.GetTime();
            var scenarios = scenarioCases.ToArray();
            OnBeforeScenarioGroup(startTime, scenarioCases.Key, scenarios);
            var results = await Task.WhenAll(scenarios.Select(s => ExecuteScenario(featureInfo, s, ctx)));
            var endTime = ctx.Timer.GetTime();
            OnAfterScenarioGroup(endTime, results);
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
            object? fixture = null;
            var scenarioInfo = CreateScenarioInfo(featureInfo, scenario, ctx);
            IScenarioResult scenarioResult = new ScenarioResult(scenarioInfo);
            EventTime startTime = ctx.Timer.GetTime();
            try
            {
                OnBeforeScenario(startTime, scenarioInfo, scenario);

                if (ctx.GlobalSetUpException != null)
                    return ScenarioResult.CreateFailed(scenarioInfo, ctx.GlobalSetUpException);
                if (ctx.CancellationToken.IsCancellationRequested)
                    return ScenarioResult.CreateIgnored(scenarioInfo, "Execution aborted");

                fixture = CreateInstance(scenario.FeatureFixtureType);
                TestContextProvider.Initialize(scenario.ScenarioMethod, scenario.ScenarioArguments);
                ScenarioBuilderContext.SetCurrent(new ScenarioBuilder(featureInfo, fixture, ctx.Integration, ctx.ExceptionProcessor, x => scenarioResult = x)
                    .WithScenarioDetails(scenarioInfo));

                var result = scenario.ScenarioMethod.Invoke(fixture, scenario.ScenarioArguments);
                //TODO: improve?
                if (result is Task taskResult)
                    await taskResult;
            }
            catch (Exception ex)
            {
                if (scenarioResult.Status == ExecutionStatus.NotRun)
                    scenarioResult = ScenarioResult.CreateFailed(scenarioInfo, ex);
            }
            finally
            {
                ScenarioBuilderContext.SetCurrent(null);
                TestContextProvider.Clear();
                (fixture as IDisposable)?.Dispose();
                OnAfterScenario(startTime, scenarioResult);
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

        private static ScenarioInfo CreateScenarioInfo(IFeatureInfo featureInfo, ScenarioCase scenario, Context ctx)
        {
            return new ScenarioInfo(featureInfo,
                ctx.MetadataProvider.GetScenarioName(new ScenarioDescriptor(scenario.ScenarioMethod, scenario.ScenarioArguments)),
                ctx.MetadataProvider.GetScenarioLabels(scenario.ScenarioMethod),
                ctx.MetadataProvider.GetScenarioCategories(scenario.ScenarioMethod),
                scenario.RuntimeId);
        }

        private object CreateInstance(TypeInfo featureFixtureType)
        {
            return Activator.CreateInstance(featureFixtureType) ?? throw new InvalidOperationException($"Failed to create instance of {featureFixtureType}");
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
            cfg.Seal();
            return cfg;
        }

        private class Context : IDisposable
        {
            public Context(Assembly testAssembly, LightBddConfiguration configuration, CancellationToken cancellationToken)
            {
                Configuration = configuration;
                CancellationToken = cancellationToken;
                MetadataProvider = new MetadataProvider(testAssembly, configuration);
                Integration = new CoreIntegrationContext(this);
                ExceptionProcessor = new ExceptionProcessor(Integration);
            }
            public readonly LightBddConfiguration Configuration;
            public readonly CancellationToken CancellationToken;
            public readonly IExecutionTimer Timer = DefaultExecutionTimer.StartNew();
            public readonly CoreMetadataProvider MetadataProvider;
            //TODO: remove
            public readonly IntegrationContext Integration;
            public readonly ExceptionProcessor ExceptionProcessor;
            public IDependencyContainer DependencyContainer => Configuration.Get<DependencyContainerConfiguration>().DependencyContainer;
            public GlobalSetUpRegistry GlobalSetUp => Configuration.Get<ExecutionExtensionsConfiguration>().GlobalSetUpRegistry;
            public IProgressNotifier ProgressNotifier => Configuration.Get<ProgressNotifierConfiguration>().Notifier;
            public Exception? GlobalSetUpException;

            public void Dispose()
            {
                DependencyContainer.Dispose();
            }
        }

        //TODO: remove
        private class CoreIntegrationContext : IntegrationContext
        {
            private readonly Context _ctx;

            public CoreIntegrationContext(Context ctx)
            {
                _ctx = ctx;
            }

            public override CoreMetadataProvider MetadataProvider => _ctx.MetadataProvider;
            public override INameFormatter NameFormatter => _ctx.Configuration.NameFormatterConfiguration().GetFormatter();
            public override Func<Exception, ExecutionStatus> ExceptionToStatusMapper { get; } = MapExceptionToStatus;
            [Obsolete] public override IFeatureProgressNotifier FeatureProgressNotifier => throw new NotImplementedException();
            [Obsolete] public override Func<object, IScenarioProgressNotifier> ScenarioProgressNotifierProvider => throw new NotImplementedException();
            public override IExecutionExtensions ExecutionExtensions => _ctx.Configuration.ExecutionExtensionsConfiguration();
            public override LightBddConfiguration Configuration => _ctx.Configuration;
            public override IDependencyContainer DependencyContainer => _ctx.Configuration.DependencyContainerConfiguration().DependencyContainer;
            public override ValueFormattingService ValueFormattingService => MetadataProvider.ValueFormattingService;
            protected override IProgressNotifier GetProgressNotifier() => _ctx.Configuration.Get<ProgressNotifierConfiguration>().Notifier;
            private static ExecutionStatus MapExceptionToStatus(Exception ex) => ex is IgnoreScenarioException ? ExecutionStatus.Ignored : ExecutionStatus.Failed;
        }
    }
}
