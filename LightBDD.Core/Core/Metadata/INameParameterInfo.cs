namespace LightBDD.Core.Metadata
{
    public interface INameParameterInfo
    {
        bool IsEvaluated { get; }
        string FormattedValue { get; }
    }
}