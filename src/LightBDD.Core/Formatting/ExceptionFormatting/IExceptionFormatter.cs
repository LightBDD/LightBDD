using System;

namespace LightBDD.Core.Formatting.ExceptionFormatting;

/// <summary>
/// Exception formatter interface
/// </summary>
public interface IExceptionFormatter
{
    /// <summary>
    /// Formats the exception by returning exception type name and message, followed by inner exception chain, followed by call stack information.
    /// </summary>
    /// <param name="exception">Exception to format</param>
    /// <returns>Formatted exception details.</returns>
    string Format(Exception exception);
}