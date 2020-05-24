using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.XUnit2.Implementation.Customization
{
    /// <summary>
    /// Based on XunitTheoryTestCaseRunner (https://github.com/xunit/xunit/blob/master/src/xunit.execution/Sdk/Frameworks/Runners/XunitTheoryTestCaseRunner.cs).
    /// The difference is that it instantiates <see cref="ScenarioTestRunner"/> and supports parallel execution.
    /// </summary>
    internal class ScenarioMultiTestCaseRunner : XunitTestCaseRunner
    {
        private static readonly object[] NoArguments = new object[0];
        private readonly ExceptionAggregator _cleanupAggregator = new ExceptionAggregator();
        private Exception _dataDiscoveryException;
        private readonly List<ScenarioTestRunner> _testRunners = new List<ScenarioTestRunner>();
        private readonly List<IDisposable> _toDispose = new List<IDisposable>();
        private readonly IMessageSink _diagnosticMessageSink;

        public ScenarioMultiTestCaseRunner(IXunitTestCase testCase, string displayName, string skipReason, object[] constructorArguments, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
            : base(testCase, displayName, skipReason, constructorArguments, NoArguments, messageBus, aggregator, cancellationTokenSource)
        {
            _diagnosticMessageSink = diagnosticMessageSink;
        }

        protected override async Task AfterTestCaseStartingAsync()
        {
            await base.AfterTestCaseStartingAsync();

            try
            {
                var dataAttributes = TestCase.TestMethod.Method.GetCustomAttributes(typeof(DataAttribute));

                foreach (var dataAttribute in dataAttributes)
                {
                    var discovererAttribute = dataAttribute.GetCustomAttributes(typeof(DataDiscovererAttribute)).First();
                    var args = discovererAttribute.GetConstructorArguments().Cast<string>().ToList();
                    var discovererType = LoadType(args[1], args[0]);
                    if (discovererType == null)
                    {
                        if (dataAttribute is IReflectionAttributeInfo reflectionAttribute)
                            Aggregator.Add(new InvalidOperationException($"Data discoverer specified for {reflectionAttribute.Attribute.GetType()} on {TestCase.TestMethod.TestClass.Class.Name}.{TestCase.TestMethod.Method.Name} does not exist."));
                        else
                            Aggregator.Add(new InvalidOperationException($"A data discoverer specified on {TestCase.TestMethod.TestClass.Class.Name}.{TestCase.TestMethod.Method.Name} does not exist."));

                        continue;
                    }

                    IDataDiscoverer discoverer;
                    try
                    {
                        discoverer = ExtensibilityPointFactory.GetDataDiscoverer(_diagnosticMessageSink, discovererType);
                    }
                    catch (InvalidCastException)
                    {
                        if (dataAttribute is IReflectionAttributeInfo reflectionAttribute)
                            Aggregator.Add(new InvalidOperationException($"Data discoverer specified for {reflectionAttribute.Attribute.GetType()} on {TestCase.TestMethod.TestClass.Class.Name}.{TestCase.TestMethod.Method.Name} does not implement IDataDiscoverer."));
                        else
                            Aggregator.Add(new InvalidOperationException($"A data discoverer specified on {TestCase.TestMethod.TestClass.Class.Name}.{TestCase.TestMethod.Method.Name} does not implement IDataDiscoverer."));

                        continue;
                    }

                    var data = discoverer.GetData(dataAttribute, TestCase.TestMethod.Method);
                    if (data == null)
                    {
                        Aggregator.Add(new InvalidOperationException($"Test data returned null for {TestCase.TestMethod.TestClass.Class.Name}.{TestCase.TestMethod.Method.Name}. Make sure it is statically initialized before this test method is called."));
                        continue;
                    }

                    foreach (var dataRow in data)
                    {
                        _toDispose.AddRange(dataRow.OfType<IDisposable>());

                        ITypeInfo[] resolvedTypes = null;
                        var methodToRun = TestMethod;
                        var convertedDataRow = methodToRun.ResolveMethodArguments(dataRow);

                        if (methodToRun.IsGenericMethodDefinition)
                        {
                            resolvedTypes = TestCase.TestMethod.Method.ResolveGenericTypes(convertedDataRow);
                            methodToRun = methodToRun.MakeGenericMethod(resolvedTypes.Select(t => ((IReflectionTypeInfo)t).Type).ToArray());
                        }

                        var parameterTypes = methodToRun.GetParameters().Select(p => p.ParameterType).ToArray();
                        convertedDataRow = Reflector.ConvertArguments(convertedDataRow, parameterTypes);

                        var theoryDisplayName = TestCase.TestMethod.Method.GetDisplayNameWithArguments(DisplayName, convertedDataRow, resolvedTypes);
                        var test = CreateTest(TestCase, theoryDisplayName);
                        var skipReason = SkipReason ?? dataAttribute.GetNamedArgument<string>("Skip");
                        _testRunners.Add(new ScenarioTestRunner(test, MessageBus, TestClass, ConstructorArguments, methodToRun, convertedDataRow, skipReason, BeforeAfterAttributes, new ExceptionAggregator(Aggregator), CancellationTokenSource));
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

            var runSummary = await TaskExecutor.RunAsync(
                                CancellationTokenSource.Token,
                                _testRunners.Select(r => (Func<Task<RunSummary>>)r.RunScenarioAsync).ToArray(),
                                TestCase.TestMethod.TestClass);

            // Run the cleanup here so we can include cleanup time in the run summary,
            // but save any exceptions so we can surface them during the cleanup phase,
            // so they get properly reported as test case cleanup failures.
            var timer = new ExecutionTimer();
            foreach (var disposable in _toDispose)
                timer.Aggregate(() => _cleanupAggregator.Run(disposable.Dispose));

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
                var asm = new AssemblyName(assemblyName);
                var assembly = Assembly.Load(new AssemblyName { Name = asm.Name, Version = asm.Version });
                return assembly.GetType(typeName);
            }
            catch
            {
                return null;
            }
        }
    }
}