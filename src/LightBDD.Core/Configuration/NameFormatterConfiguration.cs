using LightBDD.Core.Formatting;
using System;

namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// Configuration class allowing to customize name formatting behavior.
    /// </summary>
    public class NameFormatterConfiguration : FeatureConfiguration
    {
        private INameFormatter _formatter;

        /// <summary>
        /// Returns formatter.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when formatter was not set.</exception>
        public INameFormatter GetFormatter() => _formatter ?? throw new InvalidOperationException($"{nameof(INameFormatter)} was not specified.");

        /// <summary>
        /// Sets <paramref name="formatter"/> as a default formatter to be used by LightBDD. The formatter can be retrieved by <see cref="GetFormatter"/>() method call.
        /// </summary>
        /// <param name="formatter">New formatter to set.</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="formatter"/> is null.</exception>
        public NameFormatterConfiguration UpdateFormatter(INameFormatter formatter)
        {
            ThrowIfSealed();
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            return this;
        }
    }
}