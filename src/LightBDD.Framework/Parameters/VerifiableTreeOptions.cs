#nullable enable
namespace LightBDD.Framework.Parameters;

public class VerifiableTreeOptions
{
    public bool CheckValueNodeTypes { get; set; } = false;
    public bool CheckObjectNodeTypes { get; set; } = false;
    public bool CheckArrayNodeTypes { get; set; } = false;
    public UnexpectedValueVerificationAction UnexpectedNodeAction { get; set; } = UnexpectedValueVerificationAction.Fail;
}