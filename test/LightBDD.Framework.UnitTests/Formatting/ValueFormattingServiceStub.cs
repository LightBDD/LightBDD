using System.Globalization;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.UnitTests.Formatting
{
    class ValueFormattingServiceStub : IValueFormattingService
    {
        private readonly CultureInfo _cultureInfo;

        public ValueFormattingServiceStub(CultureInfo cultureInfo)
        {
            _cultureInfo = cultureInfo;
        }

        public string FormatValue(object value)
        {
            return string.Format(GetCultureInfo(), "{0}", value);
        }

        public CultureInfo GetCultureInfo() => _cultureInfo;
    }
}