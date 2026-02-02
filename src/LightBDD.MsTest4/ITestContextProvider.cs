using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.MsTest4
{
    /// <summary>
    /// An interface allowing to retrieve <see cref="Microsoft.VisualStudio.TestTools.UnitTesting.TestContext"/> associated to the test class instance.
    /// </summary>
    public interface ITestContextProvider
    {
        /// <summary>
        /// Returns <see cref="Microsoft.VisualStudio.TestTools.UnitTesting.TestContext"/> associated to the test class instance.
        /// </summary>
        TestContext TestContext { get; }
    }
}