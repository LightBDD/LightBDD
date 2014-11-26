using System.Collections;
using System.Linq;

namespace LightBDD.AcceptanceTests.Helpers
{
    internal class ArrayFormatAttribute : ParameterFormatterAttribute
    {
        public override string Format(object parameter)
        {
            return string.Join(", ", ((IEnumerable)parameter).OfType<object>());
        }
    }
}