namespace LightBDD.Core.Metadata
{
    /// <summary>
    /// Interface describing name parameter metadata.
    /// </summary>
    public interface INameParameterInfo
    {
        /// <summary>
        /// Returns <c>true</c> if parameter is already evaluated or <c>false</c> if not.
        /// </summary>
        bool IsEvaluated { get; }
        /// <summary>
        /// Returns verification status.
        /// </summary>
        ParameterVerificationStatus VerificationStatus { get; }
        /// <summary>
        /// Returns formatted parameter value.
        /// </summary>
        string FormattedValue { get; }
    }
}