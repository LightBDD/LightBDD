using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.XUnit
{
    /// <summary>
    /// Based on XunitTheoryTestCaseRunner (https://github.com/xunit/xunit/blob/master/src/xunit.execution/Sdk/Frameworks/Runners/XunitTheoryTestCaseRunner.cs).
    /// The difference is that it supports both, Fact and Theory behaviour and uses ScenarioTestRunner
    /// </summary>
    internal class ScenarioTestCaseRunner : XunitTestCaseRunner
    {
        private static readonly object[] NoArguments = new object[0];
        private readonly ExceptionAggregator _cleanupAggregator = new ExceptionAggregator();
        private readonly List<ScenarioTestRunner> _testRunners = new List<ScenarioTestRunner>();
        private readonly List<IDisposable> _toDispose = new List<IDisposable>();
        private Exception _dataDiscoveryException;
        private readonly IMessageSink _diagnosticMessageSink;

        public ScenarioTestCaseRunner(IXunitTestCase testCase, string displayName, string skipReason, object[] constructorArguments, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            : base(testCase, displayName, skipReason, constructorArguments, NoArguments, messageBus, aggregator, cancellationTokenSource)
        {
            _diagnosticMessageSink = diagnosticMessageSink;
        }

        protected override async Task AfterTestCaseStartingAsync()
        {
            await base.AfterTestCaseStartingAsync();
            CreateTheorySpecificTestRunners();
            if (!_testRunners.Any())
                _testRunners.Add(new ScenarioTestRunner(new XunitTest(TestCase, DisplayName), MessageBus, TestClass, ConstructorArguments, TestMethod, TestMethodArguments, SkipReason, BeforeAfterAttributes, new ExceptionAggregator(Aggregator), CancellationTokenSource));

        }

        private void CreateTheorySpecificTestRunners()
        {
            try
            {
                var dataAttributes = TestCase.TestMethod.Method.GetCustomAttributes(typeof(DataAttribute));

                foreach (var dataAttribute in dataAttributes)
                {
                    var discovererAttribute = dataAttribute.GetCustomAttributes(typeof(DataDiscovererAttribute)).First();
                    var args = discovererAttribute.GetConstructorArguments().Cast<string>().ToList();
                    var discovererType = LoadType(args[1], args[0]);
                    var discoverer = ExtensibilityPointFactory.GetDataDiscoverer(_diagnosticMessageSink, discovererType);

                    foreach (var dataRow in discoverer.GetData(dataAttribute, TestCase.TestMethod.Method))
                    {
                        _toDispose.AddRange(dataRow.OfType<IDisposable>());

                        ITypeInfo[] resolvedTypes = null;
                        var methodToRun = TestMethod;

                        if (methodToRun.IsGenericMethodDefinition)
                        {
                            resolvedTypes = TestCase.TestMethod.Method.ResolveGenericTypes(dataRow);
                            methodToRun = methodToRun.MakeGenericMethod(resolvedTypes.Select(t => ((IReflectionTypeInfo)t).Type).ToArray());
                        }

                        var parameterTypes = methodToRun.GetParameters().Select(p => p.ParameterType).ToArray();
                        var convertedDataRow = Reflector.ConvertArguments(dataRow, parameterTypes);
                        var theoryDisplayName = TestCase.TestMethod.Method.GetDisplayNameWithArguments(DisplayName, convertedDataRow, resolvedTypes);
                        var test = new XunitTest(TestCase, theoryDisplayName);

                        _testRunners.Add(new ScenarioTestRunner(test, MessageBus, TestClass, ConstructorArguments, methodToRun,
                            convertedDataRow, SkipReason, BeforeAfterAttributes, Aggregator, CancellationTokenSource));
                    }
                }
            }
            catch (Exception ex)
            {
                // Stash the exception so we can surface it during RunTestAsync
                _dataDiscoveryException = ex;
            }
        }

        protected override Task BeforeTestCaseFinishedAsync()
        {
            Aggregator.Aggregate(_cleanupAggregator);

            return base.BeforeTestCaseFinishedAsync();
        }

        protected override async Task<RunSummary> RunTestAsync()
        {
            if (_dataDiscoveryException != null)
                return RunTest_DataDiscoveryException();

            var runSummary = new RunSummary();
            foreach (var testRunner in _testRunners)
                runSummary.Aggregate(await testRunner.RunScenarioAsync());

            // Run the cleanup here so we can include cleanup time in the run summary,
            // but save any exceptions so we can surface them during the cleanup phase,
            // so they get properly reported as test case cleanup failures.
            var timer = new ExecutionTimer();
            foreach (var disposable in _toDispose)
                timer.Aggregate(() => _cleanupAggregator.Run(() => disposable.Dispose()));

            runSummary.Time += timer.Total;
            return runSummary;
        }

        RunSummary RunTest_DataDiscoveryException()
        {
            var test = new XunitTest(TestCase, DisplayName);

            if (!MessageBus.QueueMessage(new TestStarting(test)))
                CancellationTokenSource.Cancel();
            else if (!MessageBus.QueueMessage(new TestFailed(test, 0, null, _dataDiscoveryException.Unwrap())))
                CancellationTokenSource.Cancel();
            if (!MessageBus.QueueMessage(new TestFinished(test, 0, null)))
                CancellationTokenSource.Cancel();

            return new RunSummary { Total = 1, Failed = 1 };
        }

        /// <summary>
        /// Simple implementation of XUnit SerializationHelper.GetType() (https://github.com/xunit/xunit/blob/master/src/common/SerializationHelper.cs)
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        private Type LoadType(string assemblyName, string typeName)
        {
            try
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == assemblyName || a.GetName().Name == assemblyName)
                               ?? Assembly.Load(assemblyName);
                return assembly.GetType(typeName);
            }
            catch
            {
                return null;
            }
        }
    }
}