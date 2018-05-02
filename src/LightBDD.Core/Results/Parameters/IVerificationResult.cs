using System;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Results.Parameters
{
    public interface IVerificationResult
    {
        /// <summary>
        /// Returns verification exception or null, if parameter validation is successful.
        /// </summary>
        Exception Exception { get; }
        /// <summary>
        /// Returns verification status.
        /// </summary>
        ParameterVerificationStatus VerificationStatus { get; }
    }
}