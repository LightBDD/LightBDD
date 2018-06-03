using System;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Results.Parameters
{
    public interface IVerificationResult
    {
        /// <summary>
        /// Returns verification message or null, if parameter validation is successful or not applicable.
        /// </summary>
        string VerificationMessage { get; }
        /// <summary>
        /// Returns verification status.
        /// </summary>
        ParameterVerificationStatus VerificationStatus { get; }
    }
}