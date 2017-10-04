using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Formatting.Values.Implementation;

namespace LightBDD.Core.Configuration
{
    public class ValueFormattingConfiguration : FeatureConfiguration
    {
        private readonly ConcurrentDictionary<Type, IValueFormatter> _explicitFormatters = new ConcurrentDictionary<Type, IValueFormatter>();
        private readonly List<IConditionalValueFormatter> _generalFormatters = new List<IConditionalValueFormatter>();

        public ValueFormattingConfiguration()
        {
            _explicitFormatters.TryAdd(typeof(string), new AsStringFormatter());
        }

        public IReadOnlyDictionary<Type, IValueFormatter> ExplicitFormatters => new ReadOnlyDictionary<Type, IValueFormatter>(_explicitFormatters);
        public IReadOnlyList<IConditionalValueFormatter> GeneralFormatters => new ReadOnlyCollection<IConditionalValueFormatter>(_generalFormatters);

        public ValueFormattingConfiguration ClearGeneral()
        {
            ThrowIfSealed();
            _generalFormatters.Clear();
            return this;
        }

        public ValueFormattingConfiguration ClearExplicit()
        {
            ThrowIfSealed();
            _explicitFormatters.Clear();
            return this;
        }

        public ValueFormattingConfiguration RegisterExplicit(Type targetType, IValueFormatter formatter)
        {
            ThrowIfSealed();

            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));

            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            _explicitFormatters.AddOrUpdate(targetType, formatter, (key, existing) => formatter);
            return this;
        }

        public ValueFormattingConfiguration RegisterGeneral(IConditionalValueFormatter formatter)
        {
            ThrowIfSealed();
            _generalFormatters.Add(formatter ?? throw new ArgumentNullException(nameof(formatter)));
            return this;
        }
    }
}