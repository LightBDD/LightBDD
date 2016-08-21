using System;
using System.ComponentModel;
using System.Configuration;
using LightBDD.Results.Formatters;
using LightBDD.SummaryGeneration;

namespace LightBDD.Configuration
{
    /// <summary>
    /// Summary writer element allowing to associate formatter to output path.
    /// </summary>
    internal class WriterElement : ConfigurationElement
    {
        private const string TYPE_FIELD = "type";

        /// <summary>
        /// Type of IResultFormatter used by summary writer.
        /// </summary>
        [ConfigurationProperty(TYPE_FIELD, IsRequired = true)]
        [SubclassTypeValidator(typeof(ISummaryWriter))]
        [TypeConverter(typeof(TypeNameConverter))]
        public Type SummaryWriter
        {
            get { return (Type)this[TYPE_FIELD]; }
            set { this[TYPE_FIELD] = value; }
        }

    }
}