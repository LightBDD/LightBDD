using System.Globalization;
using System.Threading;
using NUnit.Framework;

namespace LightBDD.UnitTests
{
    [SetUpFixture]
    public class TestSuiteSetup
    {
        [SetUp]
        public void SetUp()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }
    }
}
