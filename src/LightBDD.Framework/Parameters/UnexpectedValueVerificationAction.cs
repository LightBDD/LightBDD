#nullable enable
using LightBDD.Core.Metadata;

namespace LightBDD.Framework.Parameters;

/// <summary>
/// Action to take when unexpected value is detected.
/// </summary>
public enum UnexpectedValueVerificationAction
{
    /// <summary>
    /// Verification will result with <seealso cref="ParameterVerificationStatus.Failure"/> status
    /// </summary>
    Fail,
    /// <summary>
    /// Verification will result with <seealso cref="ParameterVerificationStatus.NotApplicable"/> status
    /// </summary>
    Ignore, 
    /// <summary>
    /// Value will not participate in verification and will get removed from the results
    /// </summary>
    Exclude
}