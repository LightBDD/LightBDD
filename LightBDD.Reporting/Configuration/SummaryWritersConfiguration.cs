using System;
using System.Collections;
using System.Collections.Generic;
using LightBDD.Core.Configuration;
using LightBDD.Reporting.Formatters;

namespace LightBDD.Reporting.Configuration
{
    public class SummaryWritersConfiguration : IEnumerable<ISummaryWriter>, IFeatureConfiguration
    {
        private readonly List<ISummaryWriter> _summaryWriters = new List<ISummaryWriter>();

        public SummaryWritersConfiguration()
        {
            Add(new SummaryFileWriter(new XmlResultFormatter(), "~\\Reports\\FeaturesSummary.xml"));
            Add(new SummaryFileWriter(new HtmlResultFormatter(), "~\\Reports\\FeaturesSummary.html"));
        }

        public SummaryWritersConfiguration Add(ISummaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            _summaryWriters.Add(writer);
            return this;
        }

        public SummaryWritersConfiguration Remove(ISummaryWriter writer)
        {
            _summaryWriters.Remove(writer);
            return this;
        }

        public SummaryWritersConfiguration Clear()
        {
            _summaryWriters.Clear();
            return this;
        }

        public IEnumerator<ISummaryWriter> GetEnumerator()
        {
            return _summaryWriters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
