using System.Globalization;
using LightBDD.Core.Formatting.Parameters;
using LightBDD.Core.Formatting.Values;

namespace LightBDD.Framework.UnitTests.Formatting
{
    internal class ValueFormattingServiceStub : IValueFormattingService
    {
        private readonly CultureInfo _cultureInfo;
        private readonly string _itemFormat;

        public ValueFormattingServiceStub(CultureInfo cultureInfo, string itemFormat = "{0}")
        {
            _cultureInfo = cultureInfo;
            _itemFormat = itemFormat;
        }

        public string FormatValue(object value)
        {
            if (value is ISelfFormattable f)
                return f.Format(this);
            return string.Format(GetCultureInfo(), _itemFormat, value);
        }

        public CultureInfo GetCultureInfo()
        {
            return _cultureInfo;
        }

        public IFormatSymbols Symbols { get; } = new FormatSymbolsStub();
    }
}