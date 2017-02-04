using LightBDD.Core.Metadata;

namespace LightBDD.Core.Formatting.NameDecorators
{
    /// <summary>
    /// Interface decorating step name.
    /// </summary>
    public interface IStepNameDecorator : INameDecorator
    {
        /// <summary>
        /// Decorates step type name.
        /// </summary>
        string DecorateStepTypeName(IStepTypeNameInfo stepTypeName);
    }
}