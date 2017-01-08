using LightBDD.Integration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD
{
    public class ScenarioAttribute : TestMethodAttribute
    {
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