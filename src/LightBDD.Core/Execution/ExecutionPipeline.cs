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

        public async Task<ITestRunResult> Execute(IEnumerable<ScenarioCase> scenarios, CancellationToken cancellationToken = default)
        {
            var ctx = new Context(_testAssembly, Configure(), cancellationToken);
            var testRunInfo = ctx.MetadataProvider.GetTestRunInfo();

            var testRunStartTime = ctx.Timer.GetTime();
            ctx.ProgressNotifier.Notify(new TestRunStarting(testRunStartTime, testRunInfo));

            var results = await Task.WhenAll(scenarios.GroupBy(s => s.FeatureFixtureType).Select(fixture => ExecuteFeature(fixture, ctx)));

            var testRunEndTime = ctx.Timer.GetTime();
            var result = new TestRunResult(testRunInfo, testRunEndTime.GetExecutionTime(testRunStartTime), results);
            ctx.ProgressNotifier.Notify(new TestRunFinished(testRunEndTime, result));
            return result;
        }

        private async Task<IFeatureResult> ExecuteFeature(IGrouping<TypeInfo, ScenarioCase> featureScenarios, Context ctx)
        {
            var featureInfo = ctx.MetadataProvider.GetFeatureInfo(featureScenarios.Key);
            var featureStartTime = ctx.Timer.GetTime();
            ctx.ProgressNotifier.Notify(new FeatureStarting(featureStartTime, featureInfo));

            var results = await Task.WhenAll(featureScenarios.SelectMany(s => ExpandParameterizedScenarios(s, ctx))
                //group by methodinfo
                .Select(s => ExecuteScenario(featureInfo, s, ctx)));

            var featureEndTime = ctx.Timer.GetTime();
            var result = new FeatureResultV2(featureInfo, results);
            ctx.ProgressNotifier.Notify(new FeatureFinished(featureEndTime, result));
            return result;
        }

        //TODO: simplify
        private async Task<IScenarioResult> ExecuteScenario(IFeatureInfo featureInfo, ScenarioCase scenario, Context ctx)
        {
            IScenarioResult scenarioResult = null;
            object fixture = null;
            try
            {
                fixture = CreateInstance(scenario.FeatureFixtureType);
                ScenarioBuilderContext.SetCurrent(new ScenarioBuilder(featureInfo, fixture, ctx.Integration, ctx.ExceptionProcessor, x => scenarioResult = x));
                var result = scenario.ScenarioMethod.Invoke(fixture, scenario.ScenarioArguments);
                //TODO: improve?
                if (result is Task taskResult)
                    await taskResult;
            }
            catch (Exception ex)
            {
                scenarioResult ??= CreateScenarioResult(featureInfo, scenario, ex);
            }
            finally
            {
                ScenarioBuilderContext.SetCurrent(null);
                (fixture as IDisposable)?.Dispose();
            }

            return scenarioResult;
        }

        private object CreateInstance(TypeInfo featureFixtureType)
        {
            return Activator.CreateInstance(featureFixtureType);
        }

        private static ScenarioResult CreateScenarioResult(IFeatureInfo featureInfo, ScenarioCase scenario, Exception ex)
        {
            var result = new ScenarioResult(new ScenarioInfo(featureInfo,
                new NameInfo(scenario.ScenarioMethod.Name, Array.Empty<INameParameterInfo>()),
                Array.Empty<string>(), Array.Empty<string>()));
            result.UpdateException(ex);
            result.UpdateScenarioResult(ExecutionStatus.Failed,ex.Message);
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

        private class Context
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
            public override IFeatureProgressNotifier FeatureProgressNotifier =>throw new NotImplementedException();
            public override Func<object, IScenarioProgressNotifier> ScenarioProgressNotifierProvider => throw new NotImplementedException();
            public override IExecutionExtensions ExecutionExtensions => _ctx.Configuration.ExecutionExtensionsConfiguration();
            public override LightBddConfiguration Configuration => _ctx.Configuration;
            public override IDependencyContainer DependencyContainer => _ctx.Configuration.DependencyContainerConfiguration().DependencyContainer;
            public override ValueFormattingService ValueFormattingService => MetadataProvider.ValueFormattingService;
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
            throw new NotImplementedException();
        }
    }
}
