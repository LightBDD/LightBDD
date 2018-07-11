using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Expectations
{
    /// <summary>
    /// Interface representing expectation that can be used to verify against specified value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IExpectation<in T> : ISelfFormattable
    {
        /// <summary>
        /// Verifies if specified <paramref name="value"/> meets the expectation.
        /// </summary>
        /// <param name="value">Value to verify.</param>
        /// <param name="formattingService">Formatting service.</param>
        /// <returns>Expectation verification result.</returns>
        ExpectationResult Verify(T value, IValueFormattingService formattingService);
    }
}