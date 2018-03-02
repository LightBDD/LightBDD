using System;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Results
{
    /// <summary>
    /// TODO
    /// </summary>
    public interface IValueResult
    {
        /// <summary>
        /// TODO
        /// </summary>
        string Output { get; }
        /// <summary>
        /// TODO
        /// </summary>
        string Input { get; }
        /// <summary>
        /// TODO
        /// </summary>
        Exception Exception { get; }
        /// <summary>
        /// TODO
        /// </summary>
        ParameterVerificationStatus VerificationStatus { get; }
    }
}