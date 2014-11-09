using System.Diagnostics;
using LightBDD.Results;

namespace LightBDD.Naming
{
    [DebuggerStepThrough]
    internal class DefaultStepNameDecorator : IStepNameDecorator
    {
        public string DecorateStepTypeName(string stepTypeName)
        {
            return stepTypeName ?? string.Empty;
        }

        public string DecorateParameterValue(IStepParameter parameter)
        {
            return parameter.FormattedValue ?? string.Empty;
        }
    }
}