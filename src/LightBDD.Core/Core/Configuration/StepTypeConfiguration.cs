using System;
using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// Step type configuration allowing to define step types recognized by LightBDD.
    /// </summary>
    public class StepTypeConfiguration : IFeatureConfiguration
    {
        /// <summary>
        /// Default repeated step replacement: and
        /// </summary>
        public static readonly string DefaultRepeatedStepReplacement = "and";
        /// <summary>
        /// Default predefined step types: given, whent, then, setup, and
        /// </summary>
        public static readonly IEnumerable<string> DefaultPredefinedStepTypes = new[] { "given", "when", "then", "setup", "and" };

        /// <summary>
        /// Default constructor initializing configuration with <see cref="DefaultRepeatedStepReplacement"/> and <see cref="DefaultPredefinedStepTypes"/>.
        /// </summary>
        public StepTypeConfiguration()
        {
            RepeatedStepReplacement = DefaultRepeatedStepReplacement;
            PredefinedStepTypes = DefaultPredefinedStepTypes.ToArray();
        }

        /// <summary>
        /// Current value of repeated step replacement that would be used to replace consecutive steps of the same type.
        /// See also: <seealso cref="StepTypeConfiguration"/>.
        /// </summary>
        public string RepeatedStepReplacement { get; private set; }
        /// <summary>
        /// Current collection of predefined step types that would be used to recognize step type in the parsed step method name.
        /// See also: <seealso cref="StepTypeConfiguration"/>.
        /// </summary>
        public IEnumerable<string> PredefinedStepTypes { get; private set; }

        /// <summary>
        /// Updates current predefined step types.
        /// </summary>
        /// <param name="stepTypes">New step types to set.</param>
        /// <returns>Self.</returns>
        public StepTypeConfiguration UpdatePredefinedStepTypes(params string[] stepTypes)
        {
            if (stepTypes == null)
                throw new ArgumentNullException(nameof(stepTypes));
            PredefinedStepTypes = stepTypes;
            return this;
        }

        /// <summary>
        /// Updates current repeated step replacement.
        /// </summary>
        /// <param name="replacement">Replacement to set.</param>
        /// <returns>Self.</returns>
        public StepTypeConfiguration UpdateRepeatedStepReplacement(string replacement)
        {
            RepeatedStepReplacement = replacement;
            return this;
        }
    }
}