using System;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Formatting.Parameters
{
    /// <summary>
    /// Interface describing a method parameter that is verifiable.
    /// </summary>
    public interface IVerifiableParameter
    {
        /// <summary>
        /// Updates parameter with <see cref="IValueFormattingService"/> that should be used to format expected/actual values.
        /// </summary>
        /// <param name="formattingService">Formatting service to use.</param>
        void SetValueFormattingService(IValueFormattingService formattingService);
        /// <summary>
        /// Function returning validation exception or null, if parameter validation is successful.
        /// </summary>
        Exception GetValidationException();
        /// <summary>
        /// Returns verification status.
        /// </summary>
        ParameterVerificationStatus Status { get; }
    }
}