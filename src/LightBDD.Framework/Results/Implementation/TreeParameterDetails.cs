using System;
using System.Text;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters.Trees;

namespace LightBDD.Framework.Results.Implementation;

internal class TreeParameterDetails : ITreeParameterDetails
{
    public TreeParameterDetails(TreeParameterNodeResult root, bool wasActualValueSet)
    {
        Root = root;
        if (wasActualValueSet)
        {
            var messages = new StringBuilder();
            foreach (var n in root.EnumerateAll())
            {
                if (!string.IsNullOrWhiteSpace(n.VerificationMessage))
                    messages.AppendLine($"{n.Path}: {n.VerificationMessage}");
                VerificationStatus = (ParameterVerificationStatus)Math.Max((int)VerificationStatus, (int)n.VerificationStatus);
            }

            VerificationMessage = messages.ToString().TrimEnd();
            if (string.IsNullOrWhiteSpace(VerificationMessage))
                VerificationMessage = null;
        }
        else
        {
            VerificationStatus = ParameterVerificationStatus.NotProvided;
            VerificationMessage = "Actual value was not provided";
        }
    }

    public string VerificationMessage { get; }
    public ParameterVerificationStatus VerificationStatus { get; }
    public ITreeParameterNodeResult Root { get; }
}