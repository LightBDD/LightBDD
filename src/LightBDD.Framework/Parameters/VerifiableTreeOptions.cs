#nullable enable

using System.Dynamic;
using System.Text.Json;
using LightBDD.Framework.Expectations;

namespace LightBDD.Framework.Parameters;

/// <summary>
/// Options for <seealso cref="VerifiableTree"/>
/// </summary>
public class VerifiableTreeOptions
{
    /// <summary>
    /// Specifies weather value nodes have to have exactly the same type.<br/>
    /// If set to <c>false</c>, it is possible to verify objects of different types as long as their mapped values are equal with <seealso cref="object.Equals(object,object)"/>, they represents compatible numbers or the expected value is of <seealso cref="Expectation{T}"/> and actual value meets it.<br/>
    /// The default value is <c>false</c>.
    /// </summary>
    public bool CheckValueNodeTypes { get; set; } = false;
    /// <summary>
    /// Specifies weather object nodes have to have exactly the same type.<br/>
    /// If set to <c>false</c>, it is possible to verify objects of different types, i.e. mix comparisons of anonymous types, domain models, <seealso cref="ExpandoObject"/>, <seealso cref="JsonElement"/> etc.<br/>
    /// The default value is <c>false</c>.
    /// </summary>
    public bool CheckObjectNodeTypes { get; set; } = false;
    /// <summary>
    /// Specifies weather array nodes have to underlying collections of exactly the same type.<br/>
    /// If set to <c>false</c>, it is possible to verify collections of various types where the verification will pass as long as all items verification passes.<br/>
    /// The default value is <c>false</c>.
    /// </summary>
    public bool CheckArrayNodeTypes { get; set; } = false;
    /// <summary>
    /// Specifies verification behavior for unexpected nodes, i.e. ones that are not present on expected object tree but found on actual one.<br/>
    /// The default value is <seealso cref="UnexpectedValueVerificationAction.Fail"/> which means that unexpected values will fail verification.
    /// </summary>
    public UnexpectedValueVerificationAction UnexpectedNodeAction { get; set; } = UnexpectedValueVerificationAction.Fail;
}