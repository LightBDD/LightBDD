using LightBDD.Core.Formatting.NameDecorators;

namespace LightBDD.Core.Metadata
{
    public interface IStepNameInfo : INameInfo
    {
        string Format(IStepNameDecorator decorator);
        string StepTypeName { get; }
    }
}