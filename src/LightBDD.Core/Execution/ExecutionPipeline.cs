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
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Extensibility.Implementation;
using LightBDD.Core.Formatting;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;
using LightBDD.Core.Notification;
using LightBDD.Core.Notification.Events;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Implementation;

namespace LightBDD.Core.Execution
{
    public class ExecutionPipeline
    {
        private readonly Assembly _testAssembly;
        private readonly Action<LightBddConfiguration>? _onConfigure;

        public ExecutionPipeline(Assembly testAssembly, Action<LightBddConfiguration>? onConfigure = null)
        {
            _testAssembly = testAssembly;
            _onConfigure = onConfigure;
        }

        public async Task<ITestRunResult> Execute(IReadOnlyList<ScenarioCase> scenarios, CancellationToken cancellationToken = default)
        {
            using var ctx = new Context(_testAssembly, Configure(), cancellationToken);
            var testRunInfo = ctx.MetadataProvider.GetTestRunInfo();
            var testRunStartTime = ctx.Timer.GetTime();
            OnBeforeTestRunStart(testRunStartTime, testRunInfo, scenarios);
            ctx.ProgressNotifier.Notify(new TestRunStarting(testRunStartTime, testRunInfo));
            var results = await Task.WhenAll(scenarios.GroupBy(s => s.FeatureFixtureType).Select(fixture => ExecuteFeature(fixture, ctx)));

            var testRunEndTime = ctx.Timer.GetTime();
            var result = new TestRunResult(testRunInfo, testRunEndTime.GetExecutionTime(testRunStartTime), results);
            ctx.ProgressNotifier.Notify(new TestRunFinished(testRunEndTime, result));
            OnAfterTestRunFinish(testRunEndTime, result);
            return result;
        }

        protected virtual void OnAfterTestRunFinish(EventTime time, ITestRunResult result)
        {
        }

        protected virtual void OnBeforeTestRunStart(EventTime time, ITestRunInfo testRunInfo, IReadOnlyList<ScenarioCase> scenarios)
        {
        }

        private async Task<IFeatureResult> ExecuteFeature(IGrouping<TypeInfo, ScenarioCase> featureScenarios, Context ctx)
        {
            var featureInfo = ctx.MetadataProvider.GetFeatureInfo(featureScenarios.Key);
            var featureStartTime = ctx.Timer.GetTime();
            var scenarios = featureScenarios.ToArray();
            OnBeforeFeatureStart(featureStartTime, featureScenarios.Key, featureInfo, scenarios);
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

        protected virtual void OnAfterFeatureFinished(EventTime time, IFeatureResult result)
        {
        }

        protected virtual void OnBeforeFeatureStart(EventTime time, TypeInfo featureType, IFeatureInfo featureInfo, ScenarioCase[] scenarios)
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

        protected virtual void OnAfterScenarioGroup(EventTime endTime, IReadOnlyList<IScenarioResult> results)
        {
        }

        protected virtual void OnBeforeScenarioGroup(EventTime time, MethodInfo scenarioMethod, IReadOnlyList<ScenarioCase> scenarios)
        {
        }

        //TODO: simplify
        private async Task<IScenarioResult> ExecuteScenario(IFeatureInfo featureInfo, ScenarioCase scenario, Context ctx)
        {
            object fixture = null;
            var scenarioInfo = CreateScenarioInfo(featureInfo, scenario, ctx);
            IScenarioResult scenarioResult = new ScenarioResult(scenarioInfo);
            EventTime startTime = ctx.Timer.GetTime();
            try
            {
                OnBeforeScenario(startTime, scenarioInfo, scenario);
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
                    scenarioResult = CreateScenarioResult(scenarioInfo, ex);
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

        protected virtual void OnBeforeScenario(EventTime time, IScenarioInfo scenarioInfo, ScenarioCase scenarioCase)
        {
        }

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
            return Activator.CreateInstance(featureFixtureType);
        }

        private static ScenarioResult CreateScenarioResult(IScenarioInfo scenarioInfo, Exception ex)
        {
            var result = new ScenarioResult(scenarioInfo);
            result.UpdateException(ex);
            result.UpdateScenarioResult(ExecutionStatus.Failed, ex.Message);
            return result;
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
            public IProgressNotifier ProgressNotifier => Configuration.Get<ProgressNotifierConfiguration>().Notifier;

            public void Dispose()
            {
                Configuration.DependencyContainerConfiguration().DependencyContainer.Dispose();
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
            public override IFeatureProgressNotifier FeatureProgressNotifier => throw new NotImplementedException();
            public override Func<object, IScenarioProgressNotifier> ScenarioProgressNotifierProvider => throw new NotImplementedException();
            public override IExecutionExtensions ExecutionExtensions => _ctx.Configuration.ExecutionExtensionsConfiguration();
            public override LightBddConfiguration Configuration => _ctx.Configuration;
            public override IDependencyContainer DependencyContainer => _ctx.Configuration.DependencyContainerConfiguration().DependencyContainer;
            public override ValueFormattingService ValueFormattingService => MetadataProvider.ValueFormattingService;
            protected override IProgressNotifier GetProgressNotifier() => _ctx.Configuration.Get<ProgressNotifierConfiguration>().Notifier;
            private static ExecutionStatus MapExceptionToStatus(Exception ex) => ex is IgnoreScenarioException ? ExecutionStatus.Ignored : ExecutionStatus.Failed;
        }
    }

    public class ScenarioBuilderContext
    {
        private static readonly AsyncLocal<ICoreScenarioBuilder?> Builder = new();

        public static ICoreScenarioBuilder Current => Builder.Value ?? throw new InvalidOperationException("No scenario is executed at this moment");

        internal static void SetCurrent(ICoreScenarioBuilder? builder) => Builder.Value = builder;
    }

    internal class MetadataProvider : CoreMetadataProvider
    {
        private readonly TestSuite _testSuite;

        public MetadataProvider(Assembly testAssembly, LightBddConfiguration configuration) : base(configuration)
        {
            _testSuite = TestSuite.Create(testAssembly);
        }

        protected override IEnumerable<string> GetImplementationSpecificScenarioCategories(MemberInfo member) => Enumerable.Empty<string>();

        protected override string? GetImplementationSpecificFeatureDescription(Type featureType) => null;
        protected override TestSuite GetTestSuite() => _testSuite;

        public override ScenarioDescriptor CaptureCurrentScenario()
        {
            var current = TestContextProvider.Current;
            return new ScenarioDescriptor(current.TestMethod, current.TestMethodArguments);
        }
    }

    internal class TestContextProvider
    {
        private static readonly AsyncLocal<TestContextProvider?> Provider = new();
        public MethodInfo TestMethod { get; }
        public object[] TestMethodArguments { get; }

        public static TestContextProvider Current => Provider.Value ?? throw new InvalidOperationException("No scenario is executed at this moment");

        public static void Initialize(MethodInfo testMethod, object[] arguments)
        {
            Provider.Value = new TestContextProvider(testMethod, arguments);
        }

        public static void Clear()
        {
            Provider.Value = null;
        }

        private TestContextProvider(MethodInfo testMethod, object[] arguments)
        {
            TestMethod = testMethod;
            TestMethodArguments = arguments;
        }
    }
}
