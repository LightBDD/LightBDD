#nullable enable
namespace LightBDD.Framework.Parameters;

public static class Tree
{
    public static InputTree<TData> For<TData>(TData data) => new(data);
    public static InputTree<TData> For<TData>(TData data, InputTreeOptions options) => new(data, options);
    public static VerifiableTree ExpectEquivalent(object? data) => new(data);
    public static VerifiableTree ExpectStrictMatch(object? data) => Expect(data, new() { CheckArrayNodeTypes = true, CheckObjectNodeTypes = true, CheckValueNodeTypes = true, UnexpectedNodeAction = UnexpectedValueVerificationAction.Fail });
    public static VerifiableTree ExpectContaining(object? data) => Expect(data, new() { UnexpectedNodeAction = UnexpectedValueVerificationAction.Exclude });
    public static VerifiableTree ExpectAtLeastContaining(object? data) => Expect(data, new() { UnexpectedNodeAction = UnexpectedValueVerificationAction.Ignore });
    public static VerifiableTree Expect(object? data, VerifiableTreeOptions options) => new(data, options);
}