using System.Collections.Generic;

namespace LightBDD.Core.Extensibility
{
    public class StepTypeConfiguration
    {
        public static readonly StepTypeConfiguration Default = new StepTypeConfiguration("and", "given", "when", "then", "setup", "and");
        public string RepeatedStepReplacement { get; }
        public IEnumerable<string> PredefinedStepTypes { get; }

        public StepTypeConfiguration(string repeatedStepReplacement, params string[] predefinedStepTypes)
        {
            RepeatedStepReplacement = repeatedStepReplacement;
            PredefinedStepTypes = predefinedStepTypes;
        }
    }
}