#nullable enable
using System.Dynamic;
using System.Text.Json;
using LightBDD.Core.Metadata;

namespace LightBDD.Framework.Parameters;

/// <summary>
/// Static class allowing to create various types of object tree parameters.
/// </summary>
public static class Tree
{
    /// <summary>
    /// Creates <seealso cref="InputTree{TData}"/> for provided <paramref name="data"/> parameter to provide detailed insights to the object structure upon progress and results rendering.
    /// </summary>
    public static InputTree<TData> For<TData>(TData data) => new(data);

    /// <summary>
    /// Creates <seealso cref="InputTree{TData}"/> for provided <paramref name="data"/> parameter to provide detailed insights to the object structure upon progress and results rendering.
    /// </summary>
    /// <param name="data">Input data</param>
    /// <param name="options">Additional options</param>
    public static InputTree<TData> For<TData>(TData data, InputTreeOptions options) => new(data, options);

    /// <summary>
    /// Creates <seealso cref="VerifiableTree{T}"/> for provided <paramref name="expected"/> parameter which allows detailed structural verification of actual versus expected object trees.<br/>
    ///
    /// The object trees will get verified successfully if both trees are structurally equivalent, i.e. underlying object types can be different, as long as:
    /// <list type="bullet">
    ///  <item>object nodes have the same set of properties</item>
    ///  <item>array nodes have the same number of items</item>
    ///  <item>value nodes have equal values (or meet specified expectations)</item>
    ///  <item>reference nodes points to the same paths</item>
    /// </list>
    /// With this method it is possible to compare objects represented by specific models, <seealso cref="ExpandoObject"/>, <seealso cref="JsonElement"/> or anonymous types.
    /// </summary>
    /// <param name="expected">Expected object structure</param>
    public static VerifiableTree<T> ExpectEquivalent<T>(T? expected) => new(expected, VerifiableTreeOptions.EquivalentMatch);

    /// <summary>
    /// Creates <seealso cref="VerifiableTree{T}"/> for provided <paramref name="expected"/> parameter which allows detailed structural verification of actual versus expected object trees.<br/>
    ///
    /// The object trees will get verified successfully if both trees uses the same models and have equal nodes:
    /// <list type="bullet">
    ///  <item>object nodes have the same types and number of properties</item>
    ///  <item>array nodes have the same types and number of items</item>
    ///  <item>value nodes have the same types and equal values</item>
    ///  <item>reference nodes points to the same paths</item>
    /// </list>
    /// </summary>
    /// <param name="expected">Expected object structure</param>
    public static VerifiableTree<T> ExpectStrictMatch<T>(T? expected) => Expect(expected, VerifiableTreeOptions.StrictMatch);

    /// <summary>
    /// Creates <seealso cref="VerifiableTree{T}"/> for provided <paramref name="expected"/> parameter which allows detailed structural verification of actual versus expected object trees.<br/>
    ///
    /// The object trees will get verified successfully if provided actual tree contains all equivalent nodes to expected tree, i.e.:
    /// <list type="bullet">
    ///  <item>object nodes have all properties that expected nodes have</item>
    ///  <item>array nodes have at least the same number of items as expected nodes</item>
    ///  <item>value nodes have equal values (or meet specified expectations)</item>
    ///  <item>reference nodes points to the same paths</item>
    /// </list>
    /// Any additional nodes that are present on actual tree but weren't on expected tree are be excluded from verification and the results. <br/>
    /// Similarly to <seealso cref="ExpectEquivalent{T}"/> method, this method allows comparison of different types of the models.
    /// </summary>
    /// <param name="expected">Expected object structure</param>
    public static VerifiableTree<T> ExpectContaining<T>(T? expected) => Expect(expected, VerifiableTreeOptions.EquivalentMatch.WithUnexpectedNodeAction(UnexpectedValueVerificationAction.Exclude));

    /// <summary>
    /// Creates <seealso cref="VerifiableTree{T}"/> for provided <paramref name="expected"/> parameter which allows detailed structural verification of actual versus expected object trees.<br/>
    ///
    /// The object trees will get verified successfully if provided actual tree contains all equivalent nodes to expected tree, i.e.:
    /// <list type="bullet">
    ///  <item>object nodes have all properties that expected nodes have</item>
    ///  <item>array nodes have at least the same number of items as expected nodes</item>
    ///  <item>value nodes have equal values (or meet specified expectations)</item>
    ///  <item>reference nodes points to the same paths</item>
    /// </list>
    /// Any additional nodes that are present on actual tree but weren't on expected tree are not verified but are included in the results with <seealso cref="ParameterVerificationStatus.NotApplicable"/>. <br/>
    /// Similarly to <seealso cref="ExpectEquivalent{T}"/> method, this method allows comparison of different types of the models.
    /// </summary>
    /// <param name="expected">Expected object structure</param>
    public static VerifiableTree<T> ExpectAtLeastContaining<T>(T? expected) => Expect(expected, VerifiableTreeOptions.EquivalentMatch.WithUnexpectedNodeAction(UnexpectedValueVerificationAction.Ignore));

    /// <summary>
    /// Creates <seealso cref="VerifiableTree{T}"/> for provided <paramref name="expected"/> parameter which allows detailed structural verification of actual versus expected object trees.<br/>
    /// The object trees verification behavior is configured with <paramref name="options"/> parameter.
    /// </summary>
    /// <param name="expected">Expected object structure</param>
    /// <param name="options">Additional options</param>
    public static VerifiableTree<T> Expect<T>(T? expected, VerifiableTreeOptions options) => new(expected, options);
}