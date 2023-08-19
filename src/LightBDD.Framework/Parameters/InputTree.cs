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

/// <summary>
/// Type representing object tree step parameter which provides detailed insights to the object structure upon progress and results rendering.
/// </summary>
/// <typeparam name="TData"></typeparam>
public class InputTree<TData> : IComplexParameter, ISelfFormattable
{
    private readonly InputTreeOptions _options;
    private readonly ObjectTreeBuilder _treeBuilder = ObjectTreeBuilder.GetConfigured();
    private IValueFormattingService _formattingService = ValueFormattingServices.Current;
    private ITreeParameterDetails? _details;

    /// <summary>
    /// Input value
    /// </summary>
    public TData Input { get; }

    /// <summary>
    /// Creates input tree for <paramref name="input"/> object using default options.
    /// </summary>
    public InputTree(TData input) : this(input, InputTreeOptions.Default)
    {
    }

    /// <summary>
    /// Creates input tree for <paramref name="input"/> object using options represented by <paramref name="options"/> parameter.
    /// </summary>
    public InputTree(TData input, InputTreeOptions options)
    {
        _options = options;
        Input = input;
    }

    /// <summary>
    /// Implicit operator allowing to conversion of <paramref name="input"/> into <seealso cref="InputTree{TData}"/>
    /// </summary>
    public static implicit operator InputTree<TData>(TData input) => new(input);

    /// <summary>
    /// Tree parameter details.
    /// </summary>
    public ITreeParameterDetails Details => _details ??= GetDetails();

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