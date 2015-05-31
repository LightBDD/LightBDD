using System.Configuration;

namespace LightBDD.Configuration
{
    internal class StepTypesElement : ConfigurationElement
    {
        private const string PREDEFINED_FIELD = "predefined";
        private const string REPEATED_STEP_REPLACEMENT_FIELD = "repeatedStepReplacement";

        [ConfigurationProperty(PREDEFINED_FIELD, DefaultValue = "given,when,then,setup,and")]
        public string Predefined
        {
            get { return (string)this[PREDEFINED_FIELD]; }
            set { this[PREDEFINED_FIELD] = value; }
        }

        [ConfigurationProperty(REPEATED_STEP_REPLACEMENT_FIELD, DefaultValue = "and")]
        public string RepeatedStepReplacement
        {
            get { return (string)this[REPEATED_STEP_REPLACEMENT_FIELD]; }
            set { this[REPEATED_STEP_REPLACEMENT_FIELD] = value; }
        }
    }
}