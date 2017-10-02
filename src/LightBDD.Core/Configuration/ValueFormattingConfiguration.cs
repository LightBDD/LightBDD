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
        private readonly ConcurrentDictionary<Type, IValueFormatter> _strictFormatters = new ConcurrentDictionary<Type, IValueFormatter>();
        private readonly List<IConditionalValueFormatter> _generalFormatters = new List<IConditionalValueFormatter>();

        public ValueFormattingConfiguration()
        {
            _strictFormatters.TryAdd(typeof(string), new AsStringFormatter());
        }

        public IReadOnlyDictionary<Type, IValueFormatter> StrictFormatters => new ReadOnlyDictionary<Type, IValueFormatter>(_strictFormatters);
        public IReadOnlyList<IConditionalValueFormatter> GeneralFormatters => new ReadOnlyCollection<IConditionalValueFormatter>(_generalFormatters);

        public ValueFormattingConfiguration Clear()
        {
            ThrowIfSealed();
            _strictFormatters.Clear();
            _generalFormatters.Clear();
            return this;
        }

        public ValueFormattingConfiguration Register(Type targetType, IValueFormatter formatter)
        {
            ThrowIfSealed();

            if (targetType == null)
                throw new ArgumentNullException(nameof(targetType));

            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            _strictFormatters.AddOrUpdate(targetType, formatter, (key, existing) => formatter);
            return this;
        }

        public ValueFormattingConfiguration Register(IConditionalValueFormatter formatter)
        {
            ThrowIfSealed();
            _generalFormatters.Add(formatter ?? throw new ArgumentNullException(nameof(formatter)));
            return this;
        }
    }
}