using System;
using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Core.Configuration
{
    public class StepTypeConfiguration : IFeatureConfiguration
    {
        public static readonly string DefaultRepeatedStepReplacement = "and";
        public static readonly IEnumerable<string> DefaultPredefinedStepTypes = new[] { "given", "when", "then", "setup", "and" };

        public StepTypeConfiguration()
        {
            RepeatedStepReplacement = DefaultRepeatedStepReplacement;
            PredefinedStepTypes = DefaultPredefinedStepTypes.ToArray();
        }

        public string RepeatedStepReplacement { get; private set; }
        public IEnumerable<string> PredefinedStepTypes { get; private set; }

        public StepTypeConfiguration UpdatePredefinedStepTypes(params string[] stepTypes)
        {
            if (stepTypes == null)
                throw new ArgumentNullException(nameof(stepTypes));
            PredefinedStepTypes = stepTypes;
            return this;
        }

        public StepTypeConfiguration UpdateRepeatedStepReplacement(string replacement)
        {
            RepeatedStepReplacement = replacement;
            return this;
        }
    }
}