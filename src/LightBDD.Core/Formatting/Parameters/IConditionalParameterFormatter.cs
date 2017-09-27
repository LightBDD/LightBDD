using LightBDD.Core.Extensibility;

namespace LightBDD.Core.Formatting.Parameters
{
    public interface IConditionalParameterFormatter : IParameterFormatter, IOrderedAttribute
    {
        bool CanFormat(object value);
    }
}