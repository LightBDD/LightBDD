#nullable enable

using System.Dynamic;
using System.Text.Json;
using LightBDD.Framework.Expectations;

namespace LightBDD.Framework.Parameters;

/// <summary>
/// Options for <seealso cref="VerifiableTree{T}"/>. This class implements immutable pattern, thus any alterations to the options creates a new instance.
/// </summary>
public class VerifiableTreeOptions
{
    /// <summary>
    /// Strict match option with <seealso cref="CheckArrayNodeTypes"/>, <seealso cref="CheckObjectNodeTypes"/> and <seealso cref="CheckValueNodeTypes"/> being set to <c>true</c> and <seealso cref="UnexpectedNodeAction"/> set to <seealso cref="UnexpectedValueVerificationAction.Fail"/>
    /// </summary>
    public static readonly VerifiableTreeOptions StrictMatch = new VerifiableTreeOptions()
        .WithCheckArrayNodeTypes(true)
        .WithCheckObjectNodeTypes(true)
        .WithCheckValueNodeTypes(true)
        .WithUnexpectedNodeAction(UnexpectedValueVerificationAction.Fail);

    /// <summary>
    /// Equivalent match match option with <seealso cref="CheckArrayNodeTypes"/>, <seealso cref="CheckObjectNodeTypes"/> and <seealso cref="CheckValueNodeTypes"/> being set to <c>false</c> and <seealso cref="UnexpectedNodeAction"/> set to <seealso cref="UnexpectedValueVerificationAction.Fail"/>
    /// </summary>
    public static readonly VerifiableTreeOptions EquivalentMatch = new();

    /// <summary>
    /// Specifies weather value nodes have to have exactly the same type.<br/>
    /// If set to <c>false</c>, it is possible to verify objects of different types as long as their mapped values are equal with <seealso cref="object.Equals(object,object)"/>, they represents compatible numbers or the expected value is of <seealso cref="Expectation{T}"/> and actual value meets it.<br/>
    /// The default value is <c>false</c>.
    /// </summary>
    public bool CheckValueNodeTypes { get; private set; } = false;

    /// <summary>
    /// Specifies weather object nodes have to have exactly the same type.<br/>
    /// If set to <c>false</c>, it is possible to verify objects of different types, i.e. mix comparisons of anonymous types, domain models, <seealso cref="ExpandoObject"/>, <seealso cref="JsonElement"/> etc.<br/>
    /// The default value is <c>false</c>.
    /// </summary>
    public bool CheckObjectNodeTypes { get; private set; } = false;

    /// <summary>
    /// Specifies weather array nodes have to underlying collections of exactly the same type.<br/>
    /// If set to <c>false</c>, it is possible to verify collections of various types where the verification will pass as long as all items verification passes.<br/>
    /// The default value is <c>false</c>.
    /// </summary>
    public bool CheckArrayNodeTypes { get; private set; } = false;

    /// <summary>
    /// Specifies verification behavior for unexpected nodes, i.e. ones that are not present on expected object tree but found on actual one.<br/>
    /// The default value is <seealso cref="UnexpectedValueVerificationAction.Fail"/> which means that unexpected values will fail verification.
    /// </summary>
    public UnexpectedValueVerificationAction UnexpectedNodeAction { get; private set; } = UnexpectedValueVerificationAction.Fail;

    /// <summary>
    /// Updates <seealso cref="CheckValueNodeTypes"/> with <paramref name="enabled"/> value.
    /// </summary>
    public VerifiableTreeOptions WithCheckValueNodeTypes(bool enabled)
    {
        var clone = Clone();
        clone.CheckValueNodeTypes = enabled;
        return clone;
    }

    /// <summary>
    /// Updates <seealso cref="CheckObjectNodeTypes"/> with <paramref name="enabled"/> value.
    /// </summary>
    public VerifiableTreeOptions WithCheckObjectNodeTypes(bool enabled)
    {
        var clone = Clone();
        clone.CheckObjectNodeTypes = enabled;
        return clone;
    }

    /// <summary>
    /// Updates <seealso cref="CheckArrayNodeTypes"/> with <paramref name="enabled"/> value.
    /// </summary>
    public VerifiableTreeOptions WithCheckArrayNodeTypes(bool enabled)
    {
        var clone = Clone();
        clone.CheckArrayNodeTypes = enabled;
        return clone;
    }

    /// <summary>
    /// Updates <seealso cref="UnexpectedNodeAction"/> with <paramref name="action"/> value.
    /// </summary>
    public VerifiableTreeOptions WithUnexpectedNodeAction(UnexpectedValueVerificationAction action)
    {
        var clone = Clone();
        clone.UnexpectedNodeAction = action;
        return clone;
    }

    private VerifiableTreeOptions() { }
    private VerifiableTreeOptions Clone() => (VerifiableTreeOptions)MemberwiseClone();
}