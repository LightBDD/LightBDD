using System;
using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// Step type configuration allowing to define step types recognized by LightBDD.
    /// </summary>
    [InjectableConfiguration]
    public class StepTypeConfiguration : FeatureConfiguration
    {
        /// <summary>
        /// Default repeated step replacement: and
        /// </summary>
        public static readonly string DefaultRepeatedStepReplacement = "and";
        /// <summary>
        /// Default predefined step types: given, when, then, setup, and
        /// </summary>
        public static readonly IReadOnlyList<string> DefaultPredefinedStepTypes = new[] { "given", "when", "then", "setup", "and" };

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
            ThrowIfSealed();
            PredefinedStepTypes = stepTypes ?? throw new ArgumentNullException(nameof(stepTypes));
            return this;
        }

        /// <summary>
        /// Updates current repeated step replacement.
        /// </summary>
        /// <param name="replacement">Replacement to set.</param>
        /// <returns>Self.</returns>
        public StepTypeConfiguration UpdateRepeatedStepReplacement(string replacement)
        {
            ThrowIfSealed();
            RepeatedStepReplacement = replacement;
            return this;
        }

        /// <summary>
        /// Updates <see cref="UseLambdaNameAsStepType"/> with new function.
        /// </summary>
        /// <param name="useLambdaNameAsStepTypeFunction">Function to use.</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="useLambdaNameAsStepTypeFunction"/> is null.</exception>
        public StepTypeConfiguration UpdateUseLambdaNameAsStepType(Func<string, bool> useLambdaNameAsStepTypeFunction)
        {
            ThrowIfSealed();
            UseLambdaNameAsStepType = useLambdaNameAsStepTypeFunction ?? throw new ArgumentNullException(nameof(useLambdaNameAsStepTypeFunction));
            return this;
        }

        /// <summary>
        /// Function defining if given lambda parameter name should be used as step type.
        /// By default any name with length longer than 1 character will be treated as a valid step type while names with 1 character will not.
        /// </summary>
        public Func<string, bool> UseLambdaNameAsStepType { get; private set; } = name => name?.Length > 1;
    }
}