using System.Configuration;
using LightBDD.Results.Formatters;

namespace LightBDD.Configuration
{
    /// <summary>
    /// Collection of summary writers.
    /// </summary>
    internal class SummaryWriterCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Default constructor initializing collection with XmlResultFormatter/"~\\FeaturesSummary.xml" writer.
        /// </summary>
        public SummaryWriterCollection()
        {
            BaseAdd(new SummaryWriterElement { Formatter = typeof(XmlResultFormatter), Output = "~\\FeaturesSummary.xml" });
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