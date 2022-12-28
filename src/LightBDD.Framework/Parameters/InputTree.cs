#nullable enable
using System.Collections.Generic;
using LightBDD.Core.Execution;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;
using LightBDD.Core.Results.Parameters.Trees;
using LightBDD.Framework.Formatting.Values;
using LightBDD.Framework.Parameters.ObjectTrees;
using LightBDD.Framework.Results.Implementation;

namespace LightBDD.Framework.Parameters;

public class InputTree<TData> : IComplexParameter, ISelfFormattable
{
    private readonly ObjectTreeBuilder _treeBuilder = ObjectTreeBuilder.Current;
    private IValueFormattingService _formattingService = ValueFormattingServices.Current;
    private ITreeParameterDetails? _details;
    public TData Input { get; }

    public InputTree(TData input)
    {
        Input = input;
    }

    public ITreeParameterDetails Details => _details ??= GetDetails();

    IParameterDetails IComplexParameter.Details => Details;

    string ISelfFormattable.Format(IValueFormattingService formattingService) => "<tree>";

    void IComplexParameter.SetValueFormattingService(IValueFormattingService formattingService)
    {
        _formattingService = formattingService;
    }

    private ITreeParameterDetails GetDetails()
    {
        var root = _treeBuilder.Build(Input);
        var results = new Dictionary<string, TreeParameterNodeResult>();
        foreach (var node in root.EnumerateAll())
        {
            var value = _formattingService.FormatValue(node);
            var result = new TreeParameterNodeResult(node.Path, node.Node, value, value, ParameterVerificationStatus.NotApplicable, string.Empty);
            results.Add(node.Path, result);
            if (node.Parent != null)
                results[node.Parent.Path].AddChild(result);
        }

        return new TreeParameterDetails(results["$"], true);
    }
}