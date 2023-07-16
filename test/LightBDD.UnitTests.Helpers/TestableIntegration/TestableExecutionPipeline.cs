using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Discovery;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Formatting;
using NUnit.Framework.Internal;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    public class TestableExecutionPipeline : ExecutionPipeline, IDisposable
    {
        public static readonly TestableExecutionPipeline Default = new(typeof(TestableExecutionPipeline).Assembly);
        private readonly IDisposable _context;

        private TestableExecutionPipeline(Assembly testAssembly, Action<LightBddConfiguration> onConfigure = null) : base(testAssembly, cfg=>
        {
            ConfigureDefaults(cfg);
            onConfigure?.Invoke(cfg);
        })
        {
            _context = CreateContext(CancellationToken.None);
        }

        private static void ConfigureDefaults(LightBddConfiguration cfg)
        {
            cfg.ProgressNotifierConfiguration().Clear().Append(NoProgressNotifier.Default);
            cfg.NameFormatterConfiguration().UpdateFormatter(DefaultNameFormatter.Instance);
            cfg.ExecutionExtensionsConfiguration().EnableStepDecorator<StepCommentHelper>();
            cfg.ExceptionHandlingConfiguration().UpdateExceptionDetailsFormatter(ex => ex.Message);
        }

        public ICoreScenarioBuilder CreateScenarioBuilder(object fixture)
        {
            InitializeContextProvider();
            return CreateScenarioBuilder(new TestResults.TestFeatureInfo(), fixture, _context, _ => { });
        }

        public async Task<IScenarioResult> ExecuteScenario(object fixture, Func<ICoreScenarioBuilder, Task> onScenario)
        {
            InitializeContextProvider();
            IScenarioResult scenarioResult = null;

            var builder = CreateScenarioBuilder(new TestResults.TestFeatureInfo(), fixture, _context, result => scenarioResult = result);
            try { await onScenario(builder); }
            catch { }
            return scenarioResult;
        }

        public IScenarioResult ExecuteScenario(object fixture, Action<ICoreScenarioBuilder> onScenario)
        {
            InitializeContextProvider();
            IScenarioResult scenarioResult = null;

            var builder = CreateScenarioBuilder(new TestResults.TestFeatureInfo(), fixture, _context, result => scenarioResult = result);
            try { onScenario(builder); }
            catch { }
            return scenarioResult;
        }

        private void InitializeContextProvider()
        {
            var currentTest = TestExecutionContext.CurrentContext.CurrentTest;
            InitializeTestContextProvider(ScenarioCase.CreateParameterized(currentTest.TypeInfo.Type.GetTypeInfo(),
                currentTest.Method.MethodInfo, currentTest.Arguments));
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
