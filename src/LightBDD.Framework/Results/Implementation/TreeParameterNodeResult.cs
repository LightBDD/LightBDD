using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters.Trees;

namespace LightBDD.Framework.Results.Implementation;

internal class TreeParameterNodeResult : ITreeParameterNodeResult
{
    public TreeParameterNodeResult(string path, string expectation, string value, ParameterVerificationStatus verificationStatus, string verificationMessage)
    {
        Path = path;
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
}