namespace LightBDD.Core.Metadata
{
    /// <summary>
    /// Parameter verification status.
    /// </summary>
    public enum ParameterVerificationStatus
    {
        /// <summary>
        /// Indicates that given parameter is not verifiable.
        /// </summary>
        NotApplicable,
        /// <summary>
        /// Indicates successful verification.
        /// </summary>
        Success,
        /// <summary>
        /// Indicates unsuccessful verification.
        /// </summary>
        Failure,
        /// <summary>
        /// Indicates exception being thrown during verification.
        /// </summary>
        Exception,
        /// <summary>
        /// Indicates that verifiable parameter was not verified.
        /// </summary>
        NotProvided
    }
}