using Xunit.Abstractions;

namespace LightBDD.XUnit2
{
    /// <summary>
    /// An interface allowing to retrieve <see cref="ITestOutputHelper"/> associated to the test class instance.
    /// </summary>
    public interface ITestOutputProvider
    {
        /// <summary>
        /// Returns <see cref="ITestOutputHelper"/> associated to the test class instance.
        /// </summary>
        ITestOutputHelper TestOutput { get; }
    }
}