using System.Configuration;

namespace LightBDD.Configuration
{
    /// <summary>
    /// LightBDD configuration section allowing to configure feature summary writers.
    /// </summary>
    internal class LightBDDConfiguration : ConfigurationSection
    {
        private const string WRITER_FIELD = "writer";
        private const string SUMMARY_WRITERS_FIELD = "summaryWriters";
        private const string STEP_TYPES_FIELD = "stepTypes";

        public static LightBDDConfiguration GetConfiguration()
        {
            return ConfigurationManager.GetSection("lightbdd") as LightBDDConfiguration ?? new LightBDDConfiguration();
        }

        /// <summary>
        /// Returns summary writers collection.
        /// </summary>
        [ConfigurationProperty(SUMMARY_WRITERS_FIELD)]
        [ConfigurationCollection(typeof(SummaryWriterCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public SummaryWriterCollection SummaryWriters
        {
            get { return (SummaryWriterCollection)this[SUMMARY_WRITERS_FIELD]; }
        }

        [ConfigurationProperty(STEP_TYPES_FIELD)]
        public StepTypesElement StepTypes
        {
            get { return (StepTypesElement)this[STEP_TYPES_FIELD]; }
            set { this[STEP_TYPES_FIELD] = value; }
        }

        [ConfigurationProperty(WRITER_FIELD)]
        public WriterElement Writer
        {
            get { return (WriterElement)this[WRITER_FIELD]; }
            set { this[WRITER_FIELD] = value; }
        }
    }
}