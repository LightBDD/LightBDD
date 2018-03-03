using System;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Results.Parameters
{
    public interface IInlineParameterResult : IParameterResult
    {
        IValueResult Value { get; }
    }
    /// <summary>
    /// TODO
    /// </summary>
    public interface IValueResult
    {
        /// <summary>
        /// TODO
        /// </summary>
        string Value { get; }
        /// <summary>
        /// TODO
        /// </summary>
        string Expectation { get; }
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