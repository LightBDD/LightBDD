using System.Collections.Generic;
using System.Linq;
using Xunit.Sdk;
using Xunit.v3;

namespace LightBDD.XUnit3.IntegrationTests.Helpers
{
    public class RunScenariosFirst : ITestMethodOrderer
    {
        IReadOnlyCollection<TTestMethod> ITestMethodOrderer.OrderTestMethods<TTestMethod>(IReadOnlyCollection<TTestMethod> testMethods)
        {
            return testMethods
                .OrderByDescending(x => x?.MethodName?.StartsWith("Scenario") == true)
                .ToArray();
        }
    }
}
