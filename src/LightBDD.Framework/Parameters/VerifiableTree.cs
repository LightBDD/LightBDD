#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Execution;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;
using LightBDD.Core.Results.Parameters.Trees;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Formatting.Values;
using LightBDD.Framework.Parameters.ObjectTrees;
using LightBDD.Framework.Results.Implementation;

namespace LightBDD.Framework.Parameters;

public class VerifiableTree : IComplexParameter, ISelfFormattable
{
    private static readonly VerifiableTreeOptions DefaultOptions = new();
    private readonly VerifiableTreeOptions _options;
    private readonly ObjectTreeBuilder _treeBuilder = ObjectTreeBuilder.Current;
    private readonly ObjectTreeNode _expected;
    private ObjectTreeNode? _actual;
    private IValueFormattingService _formattingService = ValueFormattingServices.Current;
    private ITreeParameterDetails? _details;

    public VerifiableTree(object? expected) : this(expected, DefaultOptions)
    {
    }

    public VerifiableTree(object? expected, VerifiableTreeOptions options)
    {
        _options = options;
        _expected = _treeBuilder.Build(expected);
    }

    public void SetActual(object? actual)
    {
        if (_actual != null)
            throw new InvalidOperationException("Actual value is already set");
        _actual = _treeBuilder.Build(actual);
        _details = null;
    }

    void IComplexParameter.SetValueFormattingService(IValueFormattingService formattingService)
    {
        _formattingService = formattingService;
    }

    IParameterDetails IComplexParameter.Details => Details;
    public ITreeParameterDetails Details => _details ??= CalculateResults();

    string ISelfFormattable.Format(IValueFormattingService formattingService) => "<tree>";

    private ITreeParameterDetails CalculateResults()
    {
        var results = new Dictionary<string, TreeParameterNodeResult>();
        var actual = (_actual?.EnumerateAll() ?? Array.Empty<ObjectTreeNode>())
            .Select((n, i) => new NodeState(n, i))
            .ToDictionary(n => n.Node.Path);

        foreach (var e in _expected.EnumerateAll())
        {
            if (actual.TryGetValue(e.Path, out var a))
            {
                VerifyNodes(results, e, a.Node);
                actual.Remove(e.Path);
            }
            else ToNode(results, e, null, ParameterVerificationStatus.Failure, "Missing value");
        }

        if (_options.UnexpectedNodeAction != UnexpectedValueVerificationAction.Exclude)
        {
            foreach (var a in actual.Values.OrderBy(v => v.Index))
            {
                if (_options.UnexpectedNodeAction == UnexpectedValueVerificationAction.Fail)
                    ToNode(results, null, a.Node, ParameterVerificationStatus.Failure, "Unexpected value");
                else
                    ToNode(results, null, a.Node, ParameterVerificationStatus.NotApplicable, "Surplus value");
            }
        }
        return new TreeParameterDetails(results["$"], _actual != null);
    }

    private ExpectationResult MatchNodes(ObjectTreeNode expected, ObjectTreeNode actual)
    {
        if (expected.Kind != actual.Kind)
            return ExpectationResult.Failure("Different node types");
        return expected.Kind switch
        {
            ObjectTreeNodeKind.Array => VerifyArrays(expected.AsArray(), actual.AsArray()),
            ObjectTreeNodeKind.Value => VerifyValues(expected.AsValue(), actual.AsValue()),
            ObjectTreeNodeKind.Reference => VerifyReferences(expected.AsReference(), actual.AsReference()),
            ObjectTreeNodeKind.Object => VerifyObjects(expected.AsObject(), actual.AsObject()),
            _ => throw new NotSupportedException($"{expected.Kind} is not supported")
        };
    }
    private void VerifyNodes(Dictionary<string, TreeParameterNodeResult> results, ObjectTreeNode expected, ObjectTreeNode actual)
    {
        var result = MatchNodes(expected, actual);
        ToNode(results, expected, actual,
            result.IsValid ? ParameterVerificationStatus.Success : ParameterVerificationStatus.Failure,
            result.Message);
    }

    private ExpectationResult VerifyReferences(ObjectTreeReference expected, ObjectTreeReference actual)
    {
        return Expect.To.Equal(expected.Target.Path)
            .Verify(actual.Target.Path, _formattingService);
    }

    private ExpectationResult VerifyObjects(ObjectTreeObject expected, ObjectTreeObject actual)
    {
        if (_options.CheckObjectNodeTypes)
            return VerifyTypes(expected, actual);
        return ExpectationResult.Success;
    }

    private ExpectationResult VerifyValues(ObjectTreeValue expected, ObjectTreeValue actual)
    {
        if (_options.CheckValueNodeTypes)
        {
            var result = VerifyTypes(expected, actual);
            if (!result.IsValid)
                return result;
        }
        return GetValueExpectation(expected).Verify(actual.Value, _formattingService);
    }

    private ExpectationResult VerifyArrays(ObjectTreeArray expected, ObjectTreeArray actual)
    {
        if (_options.CheckArrayNodeTypes)
        {
            var result = VerifyTypes(expected, actual);
            if (!result.IsValid) return result;
        }

        if (_options.UnexpectedNodeAction == UnexpectedValueVerificationAction.Fail)
            return expected.Items.Count == actual.Items.Count
                ? ExpectationResult.Success
                : ExpectationResult.Failure($"Expected exactly {expected.Items.Count} items");

        return expected.Items.Count <= actual.Items.Count
            ? ExpectationResult.Success
            : ExpectationResult.Failure($"Expected at least {expected.Items.Count} items");
    }

    private ExpectationResult VerifyTypes(ObjectTreeNode expected, ObjectTreeNode actual)
    {
        var expectedType = expected.RawObject?.GetType();
        var actualType = actual.RawObject?.GetType();
        return expectedType == actualType
            ? ExpectationResult.Success
            : ExpectationResult.Failure($"expected '{_formattingService.FormatValue(expectedType)}' type, but got '{_formattingService.FormatValue(actualType)}'");
    }

    private static IExpectation<object?> GetValueExpectation(ObjectTreeValue expected)
    {
        var expectedValue = expected.Value;
        if (expectedValue is IExpectation<object?> expectation)
            return expectation;
        return Expect.To.Equal(expectedValue);
    }

    private void ToNode(Dictionary<string, TreeParameterNodeResult> results, ObjectTreeNode? expected, ObjectTreeNode? actual, ParameterVerificationStatus status, string verificationMessage)
    {
        var expectedValue = expected != null ? _formattingService.FormatValue(expected) : "<none>";
        var actualValue = actual != null ? _formattingService.FormatValue(actual) : "<none>";
        var node = expected ?? actual ?? throw new InvalidOperationException("Neither expected or actual node is provided");

        var r = new TreeParameterNodeResult(node.Path, node.Node, expectedValue, actualValue, status, verificationMessage);
        if (!results.ContainsKey(r.Path))//TODO:remove
            results.Add(r.Path, r);
        if (node.Parent != null)
            results[node.Parent.Path].AddChild(r);
    }

    class NodeState
    {
        public NodeState(ObjectTreeNode node, int index)
        {
            Node = node;
            Index = index;
        }
        public ObjectTreeNode Node { get; }
        public int Index { get; }
    }
}