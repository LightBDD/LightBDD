#nullable enable
using System;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.Parameters;

/// <summary>
/// Class holding exception that has been captured during evaluation of parameters or their descendant members.
/// </summary>
public sealed class ExceptionCapture : ISelfFormattable
{
    /// <summary>
    /// Captured exception
    /// </summary>
    public Exception Exception { get; }

    /// <summary>
    /// Default constructor accepting captured <paramref name="exception"/>.
    /// </summary>
    /// <param name="exception">Exception to capture</param>
    public ExceptionCapture(Exception exception)
    {
        Exception = exception;
    }

    /// <inheritdoc />
    public string Format(IValueFormattingService formattingService) => ToString();

    /// <inheritdoc />
    public override string ToString() => $"{Exception.GetType().Name}: {Exception.Message}";
}