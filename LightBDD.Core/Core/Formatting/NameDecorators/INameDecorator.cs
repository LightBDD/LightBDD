using LightBDD.Core.Metadata;

namespace LightBDD.Core.Formatting.NameDecorators
{
    public interface INameDecorator
    {
        /// <summary>
        /// Decorates parameter value.
        /// </summary>
        string DecorateParameterValue(INameParameterInfo parameter);

        /// <summary>
        /// Decorates name format.
        /// </summary>
        string DecorateNameFormat(string nameFormat);
    }
}