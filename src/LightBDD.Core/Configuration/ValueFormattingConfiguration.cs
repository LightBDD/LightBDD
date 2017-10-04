using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Formatting.Values.Implementation;

namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// Configuration allowing to define explicit and general value formatters that would be used to format parameters of all scenarios and steps.
    /// </summary>
    public class ValueFormattingConfiguration : FeatureConfiguration
    {
        private readonly ConcurrentDictionary<Type, IValueFormatter> _explicitFormatters = new ConcurrentDictionary<Type, IValueFormatter>();

        private readonly List<IConditionalValueFormatter> _generalFormatters = new List<IConditionalValueFormatter>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ValueFormattingConfiguration()
        {
            RegisterRequiredExplicitFormatters();
        }

        /// <summary>
        /// Returns explicit formatters that would be used to format values of type defined as a key.
        /// </summary>
        public IReadOnlyDictionary<Type, IValueFormatter> ExplicitFormatters => new ReadOnlyDictionary<Type, IValueFormatter>(_explicitFormatters);

        /// <summary>
        /// Returns general formatters that would be used to format values of any type acceptable by <see cref="IConditionalValueFormatter.CanFormat"/>() method.
        ///
        /// Please note that general formatters would be used only if there is no suitable formatter in <see cref="ExplicitFormatters"/> collection.
        /// If above condition is meet, the first suitable general formatter would be used.
        /// </summary>
        public IReadOnlyList<IConditionalValueFormatter> GeneralFormatters => new ReadOnlyCollection<IConditionalValueFormatter>(_generalFormatters);

        /// <summary>
        /// Clears all general formatters.
        /// </summary>
        /// <returns>Self.</returns>
        public ValueFormattingConfiguration ClearGeneral()
        {
            ThrowIfSealed();
            _generalFormatters.Clear();
            return this;
        }

        /// <summary>
        /// Clears all explicit formatters but registers required formatters such as formatter for <see cref="string"/> type.
        /// </summary>
        /// <returns>Self.</returns>
        public ValueFormattingConfiguration ClearExplicit()
        {
            ThrowIfSealed();
            _explicitFormatters.Clear();
            RegisterRequiredExplicitFormatters();
            return this;
        }

        /// <summary>
        /// Registers the explicit formatter specified by <paramref name="formatter"/> parameter that would be used to format values of <paramref name="targetType"/> type.
        /// If there is already an explicit formatter specified for <paramref name="targetType"/> type, it would be overriden with new formatter.
        /// </summary>
        /// <param name="targetType">Type that would be formatted.</param>
        /// <param name="formatter">Formatter used to format given type.</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="targetType"/> or <paramref name="formatter"/> is null.</exception>
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

        /// <summary>
        /// Registers the general formatter specified by <paramref name="formatter"/> parameter that would be used to format values of any type acceptable by <see cref="IConditionalValueFormatter.CanFormat"/>() method.
        ///
        /// Please note that general formatters would be used only if there is no suitable formatter in <see cref="ExplicitFormatters"/> collection.
        /// If above condition is meet, the first suitable general formatter would be used.
        /// </summary>
        /// <param name="formatter"></param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatter"/> is null.</exception>
        public ValueFormattingConfiguration RegisterGeneral(IConditionalValueFormatter formatter)
        {
            ThrowIfSealed();
            _generalFormatters.Add(formatter ?? throw new ArgumentNullException(nameof(formatter)));
            return this;
        }

        private void RegisterRequiredExplicitFormatters()
        {
            _explicitFormatters.TryAdd(typeof(string), new AsStringFormatter());
        }
    }
}