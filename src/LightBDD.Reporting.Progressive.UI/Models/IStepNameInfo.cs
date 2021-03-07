namespace LightBDD.Reporting.Progressive.UI.Models
{
    public interface IStepNameInfo : INameInfo
    {
        string OriginalTypeName { get; }
        string TypeName { get; }
    }
}