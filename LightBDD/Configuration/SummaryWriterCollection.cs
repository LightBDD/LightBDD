using System;
using System.ComponentModel;
using System.Configuration;
using LightBDD.Results.Formatters;
using LightBDD.SummaryGeneration;

namespace LightBDD.Configuration
{
    /// <summary>
    /// Collection of summary writers.
    /// </summary>
    internal class SummaryWriterCollection : ConfigurationElementCollection
    {
        private const string TYPE_FIELD = "type";

        /// <summary>
        /// Default constructor initializing collection with XmlResultFormatter/"~\\FeaturesSummary.xml" writer.
        /// </summary>
        public SummaryWriterCollection()
        {
            BaseAdd(new SummaryWriterElement { Formatter = typeof(XmlResultFormatter), Output = "~\\FeaturesSummary.xml" });
        }

        /// <summary>
        /// Type of ISummaryWriter used to write summary files.
        /// </summary>
        [ConfigurationProperty(TYPE_FIELD, DefaultValue = typeof(SummaryFileWriter))]
        [SubclassTypeValidator(typeof(ISummaryWriter))]
        [TypeConverter(typeof(TypeNameConverter))]
        public Type Type
        {
            get { return (Type)this[TYPE_FIELD]; }
            set { this[TYPE_FIELD] = value; }
        }

        /// <summary>
        /// Returns new SummaryWriterElement element.
        /// </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new SummaryWriterElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for. </param>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SummaryWriterElement)element).Output;
        }
    }
}