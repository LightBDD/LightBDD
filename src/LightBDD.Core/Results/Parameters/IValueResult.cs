namespace LightBDD.Core.Results.Parameters
{
    /// <summary>
    /// Interface representing value result.
    /// </summary>
    public interface IValueResult : IVerificationResult
    {
        /// <summary>
        /// Returns actual value.
        /// </summary>
        string Value { get; }
        /// <summary>
        /// Returns parameter expectation.
        /// </summary>
        string Expectation { get; }
    }
}