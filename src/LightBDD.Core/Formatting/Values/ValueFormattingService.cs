using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting.Parameters.Implementation;
using LightBDD.Core.Formatting.Values.Implementation;

namespace LightBDD.Core.Formatting.Values
{
    /// <summary>
    /// Default implementation of <see cref="IValueFormattingService"/> allowing to format values basing on the provided <see cref="LightBddConfiguration"/> configuration as well as offering ability to override formatters.
    /// </summary>
    [DebuggerStepThrough]
    public class ValueFormattingService : IValueFormattingService
    {
        private readonly IConditionalValueFormatter[] _generalFormatters;
        private readonly ConcurrentDictionary<Type, IValueFormatter> _formatters;
        private readonly ICultureInfoProvider _cultureInfoProvider;

        /// <summary>
        /// Constructor initializing instance with provided configuration.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public ValueFormattingService(LightBddConfiguration configuration) : this(configuration.Get<ValueFormattingConfiguration>(), configuration.CultureInfoProviderConfiguration().CultureInfoProvider)
        { }

        /// <summary>
        /// Constructor initializing instance with provided configuration.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        /// <param name="cultureInfoProvider">CultureInfo provider.</param>
        public ValueFormattingService(ValueFormattingConfiguration configuration, ICultureInfoProvider cultureInfoProvider)
        {
            _generalFormatters = configuration.GeneralFormatters.ToArray();
            _formatters = new ConcurrentDictionary<Type, IValueFormatter>(configuration.ExplicitFormatters);
            _cultureInfoProvider = cultureInfoProvider;
        }

        /// <summary>
        /// Formats value provided by <paramref name="value"/> parameter.
        /// </summary>
        /// <param name="value">Value to format.</param>
        /// <returns>Formatted string representation of the provided value.</returns>
        public string FormatValue(object value)
        {
            return FormatValue(value, this);
        }

        private string FormatValue(object value, IValueFormattingService formattingService)
        {
            if (value == null)
                return FormatSymbols.Instance.NullValue;

            var valueFormatter = _formatters.GetOrAdd(value.GetType(), LookupFormatter);
            return valueFormatter.FormatValue(value, formattingService);
        }

        /// <summary>
        /// Returns current <see cref="CultureInfo"/> that will be used to format values.
        /// </summary>
        /// <returns>Current <see cref="CultureInfo"/> instance.</returns>
        public CultureInfo GetCultureInfo()
        {
            return _cultureInfoProvider.GetCultureInfo();
        }

        private IValueFormatter LookupFormatter(Type type)
        {
            if (SelfFormattable.Instance.CanFormat(type))
                return SelfFormattable.Instance;
            return (IValueFormatter)_generalFormatters.FirstOrDefault(f => f.CanFormat(type)) ?? DefaultValueFormatter.Instance;
        }

        /// <summary>
        /// Creates a new <see cref="IValueFormattingService"/> based on current instance and formatters specified by <paramref name="formatters"/> parameter, that new service instance will be use first.
        /// </summary>
        /// <param name="formatters">Formatters that will be used first, before falling back to ones specified in current instance of formatting service.</param>
        /// <returns></returns>
        public IValueFormattingService WithFormattersOverride(IConditionalValueFormatter[] formatters)
        {
            if (!formatters.Any())
                return this;
            return new CompositeValueFormattingService(this, formatters);
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
                    : _parent.FormatValue(value, this);
            }

            public CultureInfo GetCultureInfo()
            {
                return _parent.GetCultureInfo();
            }
        }
    }
}