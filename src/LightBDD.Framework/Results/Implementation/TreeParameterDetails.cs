using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters.Trees;

namespace LightBDD.Framework.Results.Implementation;

internal class TreeParameterDetails : ITreeParameterDetails
{
    public TreeParameterDetails(IEnumerable<TreeParameterNodeResult> nodes, bool wasActualValueSet)
    {
        Nodes = nodes.OrderBy(x => x.Path).ToArray();
        if (wasActualValueSet)
        {
            VerificationStatus = Nodes.Select(n => n.VerificationStatus)
                .DefaultIfEmpty(ParameterVerificationStatus.NotProvided).Max();
            VerificationMessage = CollectMessages();
        }
        else
        {
            VerificationStatus = ParameterVerificationStatus.NotProvided;
            VerificationMessage = "Actual value was not provided";
        }
    }

    private string CollectMessages()
    {
        var messages = Nodes.Where(n => !string.IsNullOrWhiteSpace(n.VerificationMessage))
            .Select(n => $"{n.Path}: {n.VerificationMessage}");

        var message = string.Join(Environment.NewLine, messages);
        return string.IsNullOrWhiteSpace(message)
            ? null
            : message;
    }

    public string VerificationMessage { get; }
    public ParameterVerificationStatus VerificationStatus { get; }
    public IReadOnlyList<ITreeParameterNodeResult> Nodes { get; }
}