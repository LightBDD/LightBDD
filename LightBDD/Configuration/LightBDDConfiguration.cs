using System.Configuration;

namespace LightBDD.Configuration
{
    /// <summary>
    /// LightBDD configuration section allowing to configure feature summary writers.
    /// </summary>
    internal class LightBDDConfiguration : ConfigurationSection
    {
        private const string SUMMARY_WRITERS_FIELD = "summaryWriters";

        /// <summary>
        /// Returns summary writers collection.
        /// </summary>
        [ConfigurationProperty(SUMMARY_WRITERS_FIELD)]
        [ConfigurationCollection(typeof(SummaryWriterCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public SummaryWriterCollection SummaryWriters
        {
            get { return (SummaryWriterCollection)this[SUMMARY_WRITERS_FIELD]; }
        }
    }
}