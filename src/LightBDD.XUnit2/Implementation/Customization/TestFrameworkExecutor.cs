using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.XUnit2.Implementation.Customization
{
    internal class TestFrameworkExecutor : XunitTestFrameworkExecutor
    {
        public TestFrameworkExecutor(AssemblyName assemblyName, ISourceInformationProvider sourceInformationProvider, IMessageSink diagnosticMessageSink)
            : base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
        {

        }

        protected override void RunTestCases(IEnumerable<IXunitTestCase> testCases, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions)
        {
            var assembly = GetAssembly();
            var bddScopeAttribute = GetLightBddScopeAttribute(assembly);

            var enableInterClassParallelization = ShallEnableInterClassParallelization(assembly, executionOptions);
            AssemblySettings.SetSettings(new AssemblySettings
            {
                EnableInterClassParallelization = enableInterClassParallelization,
                UseXUnitSkipBehavior = ShallUseXUnitSkipBehavior(assembly)
            });

            bddScopeAttribute?.SetUp();
            try
            {
                using (var assemblyRunner = CreateAssemblyRunner(testCases, executionMessageSink, executionOptions, enableInterClassParallelization))
                    assemblyRunner.RunAsync().Wait();
            }
            finally
            {
                bddScopeAttribute?.TearDown();
            }
        }

        private XunitTestAssemblyRunner CreateAssemblyRunner(IEnumerable<IXunitTestCase> testCases,
            IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions,
            bool enableInterClassParallelization)
        {
            return enableInterClassParallelization
                ? new TestFrameworkAssemblyRunner(TestAssembly, testCases, DiagnosticMessageSink, executionMessageSink, executionOptions)
                : new XunitTestAssemblyRunner(TestAssembly, testCases, DiagnosticMessageSink, executionMessageSink, executionOptions);
        }

        private bool ShallEnableInterClassParallelization(Assembly assembly, ITestFrameworkExecutionOptions executionOptions)
        {
            var attribute = assembly.GetCustomAttribute<CollectionBehaviorAttribute>();
            if (executionOptions.DisableParallelization() ?? attribute?.DisableTestParallelization ?? false)
                return false;
            return assembly.GetCustomAttribute<ClassCollectionBehaviorAttribute>()?.AllowTestParallelization ?? false;
        }

        private bool ShallUseXUnitSkipBehavior(Assembly assembly)
        {
            return assembly.GetCustomAttribute<UseXUnitSkipBehaviorAttribute>() != null;
        }

        private Assembly GetAssembly()
        {
            return Assembly.Load(new AssemblyName(TestAssembly.Assembly.Name));
        }
        private LightBddScopeAttribute GetLightBddScopeAttribute(Assembly assembly)
        {
            var attribs = assembly
                .GetCustomAttributes(typeof(LightBddScopeAttribute))
                .Cast<LightBddScopeAttribute>().ToArray();
            if (attribs.Length > 1)
                throw new InvalidOperationException($"Only one attribute of {typeof(LightBddScopeAttribute)} type can be defined in assembly {assembly.FullName}");
            return attribs.FirstOrDefault();
        }
    }
}