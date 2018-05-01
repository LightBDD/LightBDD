using System;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Results.Parameters
{
    /// <summary>
    /// Interface representing value result.
    /// </summary>
    public interface IValueResult
    {
        /// <summary>
        /// Returns actual value.
        /// </summary>
        string Value { get; }
        /// <summary>
        /// Returns parameter expectation.
        /// </summary>
        string Expectation { get; }
        /// <summary>
        /// Returns parameter evaluation exception, or null if none is present.
        /// </summary>
        Exception Exception { get; }
        /// <summary>
        /// Returns parameter value verification status.
        /// </summary>
        ParameterVerificationStatus VerificationStatus { get; }
    }
}