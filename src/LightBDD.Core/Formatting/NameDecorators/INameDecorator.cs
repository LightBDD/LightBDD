using LightBDD.Core.Metadata;

namespace LightBDD.Core.Formatting.NameDecorators
{
    /// <summary>
    /// Interface allowing to decorate name.
    /// </summary>
    public interface INameDecorator
    {
        /// <summary>
        /// Decorates provided parameter.
        /// </summary>
        /// <param name="parameter">Parameter to decorate.</param>
        /// <returns>Decorated parameter text.</returns>
        string DecorateParameterValue(INameParameterInfo parameter);
        /// <summary>
        /// Decorates name format.
        /// </summary>
        /// <param name="nameFormat">Name format to decorate.</param>
        /// <returns>Decorated text.</returns>
        string DecorateNameFormat(string nameFormat);
    }
}