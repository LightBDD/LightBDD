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
        var results = new List<TreeParameterNodeResult>();
        var expected = _expected.EnumerateAll().ToDictionary(e => e.Path);
        foreach (var actual in _actual?.EnumerateAll() ?? Enumerable.Empty<ObjectTreeNode>())
        {
            if (expected.TryGetValue(actual.Path, out var ex))
            {
                results.Add(VerifyNodes(ex, actual));
                expected.Remove(actual.Path);
            }
            else results.Add(ToNode(null, actual, ParameterVerificationStatus.Failure, "Unexpected value"));
        }

        foreach (var ex in expected.Values)
            results.Add(ToNode(ex, null, ParameterVerificationStatus.Failure, "Missing value"));
        return new TreeParameterDetails(results, _actual != null);
    }

    private TreeParameterNodeResult VerifyNodes(ObjectTreeNode expected, ObjectTreeNode actual)
    {
        if (expected.Kind != actual.Kind)
            return ToNode(expected, actual, ParameterVerificationStatus.Failure, "Different node types");

        switch (expected.Kind)
        {
            case ObjectTreeNodeKind.Array when expected.AsArray().Items.Count != actual.AsArray().Items.Count:
                return ToNode(expected, actual, ParameterVerificationStatus.Failure, "Different collection size");
            case ObjectTreeNodeKind.Array:
                return ToNode(expected, actual, ParameterVerificationStatus.Success, string.Empty);
            case ObjectTreeNodeKind.Value:
                {
                    var result = Expect.To.Equal(expected.AsValue().Value).Verify(actual.AsValue().Value, _formattingService);
                    return ToNode(expected, actual,
                        result.IsValid ? ParameterVerificationStatus.Success : ParameterVerificationStatus.Failure,
                        result.Message);
                }
            default:
                return ToNode(expected, actual, ParameterVerificationStatus.Success, string.Empty);
        }
    }

    private TreeParameterNodeResult ToNode(ObjectTreeNode? expected, ObjectTreeNode? actual, ParameterVerificationStatus status, string verificationMessage)
    {
        var expectedValue = expected != null ? _formattingService.FormatValue(expected) : "<none>";
        var actualValue = actual != null ? _formattingService.FormatValue(actual) : "<none>";
        var path = expected?.Path ?? actual?.Path ?? string.Empty;
        return new TreeParameterNodeResult(path, expectedValue, actualValue, status, verificationMessage);
    }
}