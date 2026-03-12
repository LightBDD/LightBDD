using Xunit.Sdk;
using Xunit.v3;

namespace LightBDD.XUnit3.Implementation.Customization
{
    internal class ScenarioFactDiscoverer : FactDiscoverer
    {
        protected override IXunitTestCase CreateTestCase(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            IXunitTestMethod testMethod,
            IFactAttribute factAttribute)
        {
            var details = TestIntrospectionHelper.GetTestCaseDetails(
                discoveryOptions,
                testMethod,
                factAttribute,
                testMethodArguments: null,
                timeout: null,
                baseDisplayName: null);

            return new ScenarioTestCase(
                details.ResolvedTestMethod,
                details.TestCaseDisplayName,
                details.UniqueID,
                details.Explicit,
                originalSkipReason: details.SkipReason,
                traits: TestIntrospectionHelper.GetTraits(testMethod, dataRow: null),
                testMethodArguments: null,
                sourceFilePath: details.SourceFilePath,
                sourceLineNumber: details.SourceLineNumber,
                timeout: details.Timeout > 0 ? details.Timeout : null);
        }
    }
}
