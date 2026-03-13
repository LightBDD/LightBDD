using System.Collections.Generic;
using System.Linq;
using Xunit.v3;

namespace LightBDD.XUnit3.IntegrationTests.Helpers
{
    public class RunScenariosFirst : ITestCaseOrderer
    {
        IReadOnlyCollection<TTestCase> ITestCaseOrderer.OrderTestCases<TTestCase>(IReadOnlyCollection<TTestCase> testCases)
        {
            return testCases
                .OrderByDescending(x => (x as IXunitTestCase)?.TestMethodName?.StartsWith("Scenario") == true)
                .ToArray();
        }
    }
}
