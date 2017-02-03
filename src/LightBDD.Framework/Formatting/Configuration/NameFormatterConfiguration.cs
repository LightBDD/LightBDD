using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Formatting;

namespace LightBDD.Framework.Formatting.Configuration
{
    /// <summary>
    /// Configuration class allowing to customize name formatting behavior.
    /// </summary>
    public class NameFormatterConfiguration : IFeatureConfiguration
    {
        /// <summary>
        /// Returns formatter.
        /// By default it is initialized with <see cref="DefaultNameFormatter"/> instance.
        /// </summary>
        public INameFormatter Formatter { get; private set; } = new DefaultNameFormatter();

        /// <summary>
        /// Updates <see cref="Formatter"/> with new value.
        /// </summary>
        /// <param name="formatter">New formatter to set.</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="formatter"/> is null.</exception>
        public NameFormatterConfiguration UpdateFormatter(INameFormatter formatter)
        {
            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));
            Formatter = formatter;
            return this;
        }
    }
}