namespace LightBDD.Core.Metadata
{
    public interface IStepInfo
    {
        IStepNameInfo Name { get; }
        int Number { get; }
        int Total { get; }
    }
}