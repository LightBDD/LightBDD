using LightBDD.MsTest2.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.MsTest2
{
    /// <summary>
    /// Scenario attribute that should be used for MsTest framework tests, as a replacement for [TestMethod] attribute.
    /// The ScenarioAttribute represents scenario test method.
    /// </summary>
    public class ScenarioAttribute : TestMethodAttribute
    {
        /// <summary>Performs scenario test.</summary>
        /// <param name="testMethod"> TestMethod for execution. </param>
        /// <returns> Test Results.</returns>
        public override TestResult[] Execute(ITestMethod testMethod)
        {
            try
            {
                TestMethodInfoProvider.TestMethod = testMethod.MethodInfo;
                return base.Execute(testMethod);
            }
            finally
            {
                TestMethodInfoProvider.TestMethod = null;
            }
        }
    }
}