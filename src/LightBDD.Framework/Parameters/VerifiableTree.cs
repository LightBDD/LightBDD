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
    private readonly VerifiableTreeOptions _options;
    private readonly ObjectTreeBuilder _treeBuilder = ObjectTreeBuilder.Current;
    private readonly ObjectTreeNode _expected;
    private ObjectTreeNode? _actual;
    private IValueFormattingService _formattingService = ValueFormattingServices.Current;
    private ITreeParameterDetails? _details;

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
        var actual = (_actual?.EnumerateAll() ?? Array.Empty<ObjectTreeNode>()).Select((n, i) => Tuple.Create(n, i)).ToDictionary(n => n.Item1.Path);
        foreach (var e in _expected.EnumerateAll())
        {
            if (actual.TryGetValue(e.Path, out var a))
            {
                VerifyNodes(results, e, a.Item1);
                actual.Remove(e.Path);
            }
            else ToNode(results, e, null, ParameterVerificationStatus.Failure, "Missing value");
        }

        foreach (var a in actual.Values.OrderBy(v => v.Item2))
            ToNode(results, null, a.Item1, ParameterVerificationStatus.Failure, "Unexpected value");
        return new TreeParameterDetails(results["$"], _actual != null);
    }


    private void VerifyNodes(Dictionary<string, TreeParameterNodeResult> results, ObjectTreeNode expected, ObjectTreeNode actual)
    {
        if (expected.Kind != actual.Kind)
            ToNode(results, expected, actual, ParameterVerificationStatus.Failure, "Different node types");
        else
        {
            switch (expected.Kind)
            {
                case ObjectTreeNodeKind.Array when expected.AsArray().Items.Count != actual.AsArray().Items.Count:
                    ToNode(results, expected, actual, ParameterVerificationStatus.Failure, "Different collection size");
                    break;
                case ObjectTreeNodeKind.Array:
                    ToNode(results, expected, actual, ParameterVerificationStatus.Success, string.Empty);
                    break;
                case ObjectTreeNodeKind.Value:
                {
                    var result = GetExpectation(expected).Verify(actual.AsValue().Value, _formattingService);

                    ToNode(results, expected, actual,
                        result.IsValid ? ParameterVerificationStatus.Success : ParameterVerificationStatus.Failure,
                        result.Message);
                    break;
                }
                case ObjectTreeNodeKind.Reference:
                {
                    var result = Expect.To.Equal(expected.AsReference().Target.Path)
                        .Verify(actual.AsReference().Target.Path, _formattingService);
                    ToNode(results, expected, actual,
                        result.IsValid ? ParameterVerificationStatus.Success : ParameterVerificationStatus.Failure,
                        result.Message);
                    break;
                }
                default:
                    ToNode(results, expected, actual, ParameterVerificationStatus.Success, string.Empty);
                    break;
            }
        }
    }

    private static IExpectation<object?> GetExpectation(ObjectTreeNode expected)
    {
        var expectedValue = expected.AsValue().Value;
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
        results.Add(r.Path, r);
        if (node.Parent != null)
            results[node.Parent.Path].AddChild(r);
    }
}