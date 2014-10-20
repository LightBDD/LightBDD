using System;
using System.ComponentModel;
using System.Configuration;
using LightBDD.Results.Formatters;

namespace LightBDD.Configuration
{
    /// <summary>
    /// Summary writer element allowing to associate formatter to output path.
    /// </summary>
    internal class SummaryWriterElement : ConfigurationElement
    {
        private const string FORMATTER_FIELD = "formatter";
        private const string OUTPUT_FIELD = "output";

        /// <summary>
        /// Type of IResultFormatter used by summary writer.
        /// </summary>
        [ConfigurationProperty(FORMATTER_FIELD, IsRequired = true)]
        [SubclassTypeValidator(typeof(IResultFormatter))]
        [TypeConverter(typeof(TypeNameConverter))]
        public Type Formatter
        {
            get { return (Type)this[FORMATTER_FIELD]; }
            set { this[FORMATTER_FIELD] = value; }
        }

        /// <summary>
        /// Output path for summary writer.
        /// </summary>
        [ConfigurationProperty(OUTPUT_FIELD, IsRequired = true, IsKey = true)]
        public string Output
        {
            get { return (string)this[OUTPUT_FIELD]; }
            set { this[OUTPUT_FIELD] = value; }
        }
    }
}