using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting.Parameters.Implementation;

namespace LightBDD.Core.Formatting.Values.Implementation
{
    internal class ValueFormattingService : IValueFormattingService
    {
        private readonly IConditionalValueFormatter[] _generalFormatters;
        private readonly ConcurrentDictionary<Type, IValueFormatter> _formatters;
        private readonly ICultureInfoProvider _cultureInfoProvider;

        public ValueFormattingService(ValueFormattingConfiguration configuration, ICultureInfoProvider cultureInfoProvider)
        {
            _generalFormatters = configuration.GeneralFormatters.ToArray();
            _formatters = new ConcurrentDictionary<Type, IValueFormatter>(configuration.ExplicitFormatters);
            _cultureInfoProvider = cultureInfoProvider;
        }

        public string FormatValue(object value)
        {
            if (value == null)
                return FormatSymbols.Instance.NullValue;

            var valueFormatter = _formatters.GetOrAdd(value.GetType(), LookupFormatter);
            return valueFormatter.FormatValue(value, this);
        }

        public CultureInfo GetCultureInfo()
        {
            return _cultureInfoProvider.GetCultureInfo();
        }

        private IValueFormatter LookupFormatter(Type type)
        {
            return (IValueFormatter)_generalFormatters.FirstOrDefault(f => f.CanFormat(type)) ?? DefaultValueFormatter.Instance;
        }

        public IValueFormattingService WithDeclaredFormatters(IConditionalValueFormatter[] declaredFormatters)
        {
            if (!declaredFormatters.Any())
                return this;
            return new CompositeValueFormattingService(this, declaredFormatters);
        }

        private class CompositeValueFormattingService : IValueFormattingService
        {
            private readonly ValueFormattingService _parent;
            private readonly IConditionalValueFormatter[] _declaredFormatters;

            public CompositeValueFormattingService(ValueFormattingService parent, IConditionalValueFormatter[] declaredFormatters)
            {
                _parent = parent;
                _declaredFormatters = declaredFormatters;
            }

            public string FormatValue(object value)
            {
                if (value == null)
                    return FormatSymbols.Instance.NullValue;

                var valueType = value.GetType();
                var declared = _declaredFormatters.FirstOrDefault(f => f.CanFormat(valueType));

                return declared != null
                    ? declared.FormatValue(value, this)
                    : _parent.FormatValue(value);
            }

            public CultureInfo GetCultureInfo() => _parent.GetCultureInfo();
        }
    }
}