using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;

namespace LightBDD.XUnit3.Implementation.Customization
{
    internal class ScenarioTheoryDiscoverer : TheoryDiscoverer
    {
        public override async ValueTask<IReadOnlyCollection<IXunitTestCase>> Discover(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            IXunitTestMethod testMethod,
            IFactAttribute factAttribute)
        {
            var theoryAttribute = (ITheoryAttribute)factAttribute;

            // If the theory attribute itself has Skip, we still want to pre-enumerate rows 
            // so each row gets the skip reason recorded in LightBDD.
            // If discovery enumeration is disabled, fall back to delay-enumerated.
            if (theoryAttribute.DisableDiscoveryEnumeration)
                return await CreateTestCasesForTheory(discoveryOptions, testMethod, theoryAttribute);

            var result = await TryPreEnumerateTestCases(discoveryOptions, testMethod, theoryAttribute);
            if (result != null)
                return result;

            // Fall back to delay-enumerated theory test case
            return await CreateTestCasesForTheory(discoveryOptions, testMethod, theoryAttribute);
        }

        private async ValueTask<IReadOnlyCollection<IXunitTestCase>> TryPreEnumerateTestCases(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            IXunitTestMethod testMethod,
            ITheoryAttribute theoryAttribute)
        {
            var testCases = new List<IXunitTestCase>();
            int? index = theoryAttribute.IncludeTestCaseIndex ? 0 : null;

            try
            {
                var disposalTracker = new DisposalTracker();

                try
                {
                    foreach (var dataAttribute in testMethod.DataAttributes)
                    {
                        if (!dataAttribute.SupportsDiscoveryEnumeration())
                            return null; // can't pre-enumerate all data, fall back

                        IReadOnlyCollection<ITheoryDataRow> dataRows;
                        try
                        {
                            dataRows = await dataAttribute.GetData(testMethod.Method, disposalTracker);
                        }
                        catch
                        {
                            return null; // data enumeration failed, fall back
                        }

                        if (dataRows == null)
                            return null;

                        foreach (var dataRow in dataRows)
                        {
                            var testMethodArguments = testMethod.ResolveMethodArguments(dataRow.GetData());
                            index++;
                            testCases.AddRange(await CreateTestCasesForDataRow(
                                discoveryOptions,
                                testMethod,
                                theoryAttribute,
                                dataRow,
                                testMethodArguments,
                                FormatTestCaseIndex(index)));
                        }
                    }
                }
                finally
                {
                    await disposalTracker.DisposeAsync();
                }
            }
            catch
            {
                return null; // any unexpected error, fall back
            }

            if (testCases.Count == 0 && theoryAttribute.SkipTestWithoutData)
            {
                var details = TestIntrospectionHelper.GetTestCaseDetails(
                    discoveryOptions,
                    testMethod,
                    (IFactAttribute)theoryAttribute,
                    testMethodArguments: null,
                    timeout: null,
                    baseDisplayName: null,
                    displayNameIndex: FormatTestCaseIndex(index + 1));

                testCases.Add(new ScenarioTestCase(
                    details.ResolvedTestMethod,
                    details.TestCaseDisplayName,
                    details.UniqueID,
                    details.Explicit,
                    originalSkipReason: "No data found for theory",
                    traits: TestIntrospectionHelper.GetTraits(testMethod, dataRow: null),
                    sourceFilePath: details.SourceFilePath,
                    sourceLineNumber: details.SourceLineNumber,
                    timeout: details.Timeout > 0 ? details.Timeout : null));
            }

            return testCases;
        }

        protected override ValueTask<IReadOnlyCollection<IXunitTestCase>> CreateTestCasesForDataRow(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            IXunitTestMethod testMethod,
            ITheoryAttribute theoryAttribute,
            ITheoryDataRow dataRow,
            object[] testMethodArguments,
            string index)
        {
            var details = TestIntrospectionHelper.GetTestCaseDetailsForTheoryDataRow(
                discoveryOptions,
                testMethod,
                theoryAttribute,
                dataRow,
                testMethodArguments,
                index);

            var testCase = new ScenarioTestCase(
                details.ResolvedTestMethod,
                details.TestCaseDisplayName,
                details.UniqueID,
                details.Explicit,
                originalSkipReason: details.SkipReason,
                traits: TestIntrospectionHelper.GetTraits(testMethod, dataRow),
                testMethodArguments: testMethodArguments,
                testLabel: dataRow.Label,
                disableParallelization: dataRow.DisableParallelization == true,
                sourceFilePath: details.SourceFilePath,
                sourceLineNumber: details.SourceLineNumber,
                timeout: details.Timeout > 0 ? details.Timeout : null);

            return new ValueTask<IReadOnlyCollection<IXunitTestCase>>(new[] { testCase });
        }

        /// <summary>Copy of xUnit's StringExtensions.FormatTestCaseIndex, which xUnit marks as internal-use only.</summary>
        private static string FormatTestCaseIndex(int? index)
        {
            return index > 0 ? "_" + index.Value.ToString("D3", CultureInfo.CurrentCulture) : null;
        }

        protected override ValueTask<IReadOnlyCollection<IXunitTestCase>> CreateTestCasesForTheory(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            IXunitTestMethod testMethod,
            ITheoryAttribute theoryAttribute)
        {
            var details = TestIntrospectionHelper.GetTestCaseDetails(
                discoveryOptions,
                testMethod,
                (IFactAttribute)theoryAttribute,
                testMethodArguments: null,
                timeout: null,
                baseDisplayName: null);

            var testCase = new ScenarioTheoryTestCase(
                details.ResolvedTestMethod,
                details.TestCaseDisplayName,
                details.UniqueID,
                details.Explicit,
                theoryAttribute.SkipTestWithoutData,
                originalSkipReason: details.SkipReason,
                traits: TestIntrospectionHelper.GetTraits(testMethod, dataRow: null),
                sourceFilePath: details.SourceFilePath,
                sourceLineNumber: details.SourceLineNumber,
                timeout: details.Timeout > 0 ? details.Timeout : null);

            return new ValueTask<IReadOnlyCollection<IXunitTestCase>>(new[] { (IXunitTestCase)testCase });
        }
    }
}
