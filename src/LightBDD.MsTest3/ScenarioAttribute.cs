using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LightBDD.MsTest3.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.MsTest3
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
            return base.Execute(new ScenarioTestMethod(testMethod));
        }
    }
}