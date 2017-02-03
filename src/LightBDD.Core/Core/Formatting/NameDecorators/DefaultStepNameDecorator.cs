using System.Diagnostics;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Formatting.NameDecorators
{
    [DebuggerStepThrough]
    internal class DefaultStepNameDecorator : IStepNameDecorator
    {
        public string DecorateStepTypeName(IStepTypeNameInfo stepTypeName)
        {
            return stepTypeName?.Name ?? string.Empty;
        }

        public string DecorateParameterValue(INameParameterInfo parameter)
        {
            return parameter.FormattedValue ?? string.Empty;
        }

        public string DecorateNameFormat(string nameFormat)
        {
            return nameFormat ?? string.Empty;
        }
    }
}