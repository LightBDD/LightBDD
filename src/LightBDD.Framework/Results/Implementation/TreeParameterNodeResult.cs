using System.Collections.Generic;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters.Trees;

namespace LightBDD.Framework.Results.Implementation;

internal class TreeParameterNodeResult : ITreeParameterNodeResult
{
    private readonly List<ITreeParameterNodeResult> _children = new();

    public TreeParameterNodeResult(string path, string node,string expectation, string value, ParameterVerificationStatus verificationStatus, string verificationMessage)
    {
        Path = path;
        Node = node;
        Expectation = expectation;
        Value = value;
        VerificationStatus = verificationStatus;
        VerificationMessage = verificationMessage;
    }

    public string VerificationMessage { get; }
    public ParameterVerificationStatus VerificationStatus { get; }
    public string Value { get; }
    public string Expectation { get; }
    public string Path { get; }
    public string Node { get; }
    public IReadOnlyList<ITreeParameterNodeResult> Children => _children;
    internal void AddChild(ITreeParameterNodeResult r) => _children.Add(r);
}