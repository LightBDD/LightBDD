using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LightBDD.MsTest4.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.MsTest4
{
    /// <summary>
    /// Scenario attribute that should be used for MsTest framework tests, as a replacement for [TestMethod] attribute.
    /// The ScenarioAttribute represents scenario test method.
    /// </summary>
    public class ScenarioAttribute : TestMethodAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScenarioAttribute"/> class.
        /// </summary>
        public ScenarioAttribute([CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
            : base(callerFilePath, callerLineNumber)
        {
        }

        /// <summary>Performs scenario test.</summary>
        /// <param name="testMethod"> TestMethod for execution. </param>
        /// <returns> Test Results.</returns>
        public override Task<TestResult[]> ExecuteAsync(ITestMethod testMethod)
        {
            return base.ExecuteAsync(new ScenarioTestMethod(testMethod));
        }
    }
}