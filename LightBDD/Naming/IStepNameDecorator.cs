using LightBDD.Results;

namespace LightBDD.Naming
{
    /// <summary>
    /// Interface decorating step name.
    /// </summary>
    public interface IStepNameDecorator
    {
        /// <summary>
        /// Decorates step type name.
        /// </summary>
        string DecorateStepTypeName(string stepTypeName);
        /// <summary>
        /// Decorates parameter value.
        /// </summary>
        string DecorateParameterValue(IStepParameter parameter);
    }
}