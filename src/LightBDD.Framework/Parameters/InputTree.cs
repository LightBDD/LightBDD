#nullable enable
using System.Collections.Generic;
using System.Linq;
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
    private static readonly InputTreeOptions DefaultOptions = new();
    private readonly InputTreeOptions _options;
    private readonly ObjectTreeBuilder _treeBuilder = ObjectTreeBuilder.Current;
    private IValueFormattingService _formattingService = ValueFormattingServices.Current;
    private ITreeParameterDetails? _details;
    public TData Input { get; }

    public InputTree(TData input) : this(input, DefaultOptions)
    {
    }

    public InputTree(TData input, InputTreeOptions options)
    {
        _options = options;
        Input = input;
    }

    public ITreeParameterDetails Details => _details ??= GetDetails();

    public static implicit operator InputTree<TData>(TData input) => new(input);

    IParameterDetails IComplexParameter.Details => Details;

    string ISelfFormattable.Format(IValueFormattingService formattingService) => "<tree>";

    void IComplexParameter.SetValueFormattingService(IValueFormattingService formattingService)
    {
        _formattingService = formattingService;
    }

    private ITreeParameterDetails GetDetails()
    {
        var results = new Dictionary<string, TreeParameterNodeResult>();
        foreach (var node in GetNodes())
        {
            var value = _formattingService.FormatValue(node);
            var result = new TreeParameterNodeResult(node.Path, node.Node, value, value, ParameterVerificationStatus.NotApplicable, string.Empty);
            results.Add(node.Path, result);
            if (node.Parent != null)
                results[node.Parent.Path].AddChild(result);
        }

        return new TreeParameterDetails(results["$"], true);
    }

    private IEnumerable<ObjectTreeNode> GetNodes()
    {
        var root = _treeBuilder.Build(Input);
        var nodes = root.EnumerateAll();
        if (_options.ExcludeNullProperties)
            nodes = nodes.Where(n => n.Kind != ObjectTreeNodeKind.Value || n.AsValue().Value is not null);
        return nodes;
    }
}