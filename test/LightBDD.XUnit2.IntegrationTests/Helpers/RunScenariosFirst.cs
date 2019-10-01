using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.XUnit2.IntegrationTests.Helpers
{
    class RunScenariosFirst : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
        {
            return testCases.OrderByDescending(x =>
                x.TestMethod.Method.GetCustomAttributes(typeof(ScenarioAttribute).AssemblyQualifiedName).Any());
        }
    }
}