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
            AssemblySettings.SetSettings(new AssemblySettings { EnableInterClassParallelization = ShallEnableInterClassParallelization(assembly, executionOptions) });
            bddScopeAttribute?.SetUp();
            try
            {
                using (var assemblyRunner = new TestFrameworkAssemblyRunner(TestAssembly, testCases, DiagnosticMessageSink, executionMessageSink, executionOptions))
                    assemblyRunner.RunAsync().Wait();
            }
            finally
            {
                bddScopeAttribute?.TearDown();
            }
        }

        private bool ShallEnableInterClassParallelization(Assembly assembly, ITestFrameworkExecutionOptions executionOptions)
        {
            var attribute = assembly.GetCustomAttribute<CollectionBehaviorAttribute>();
            if (executionOptions.DisableParallelization() ?? attribute?.DisableTestParallelization ?? false)
                return false;
            return assembly.GetCustomAttribute<ClassCollectionBehaviorAttribute>()?.AllowTestParallelization ?? false;
        }

        private Assembly GetAssembly()
        {
            var asmName = TestAssembly.Assembly.Name;
#if NET45
            return Assembly.Load(asmName);
#else
            return Assembly.Load(new AssemblyName(asmName));
#endif
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