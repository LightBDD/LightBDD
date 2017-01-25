using System;
using System.Collections;
using System.Collections.Generic;
using LightBDD.Core.Configuration;
using LightBDD.Reporting.Formatters;

namespace LightBDD.Reporting.Configuration
{
    public class ReportWritersConfiguration : IEnumerable<IReportWriter>, IFeatureConfiguration
    {
        private readonly List<IReportWriter> _summaryWriters = new List<IReportWriter>();

        public ReportWritersConfiguration()
        {
            Add(new ReportFileWriter(new XmlReportFormatter(), "~\\Reports\\FeaturesReport.xml"));
            Add(new ReportFileWriter(new HtmlReportFormatter(), "~\\Reports\\FeaturesReport.html"));
        }

        public ReportWritersConfiguration Add(IReportWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            _summaryWriters.Add(writer);
            return this;
        }

        public ReportWritersConfiguration Remove(IReportWriter writer)
        {
            _summaryWriters.Remove(writer);
            return this;
        }

        public ReportWritersConfiguration Clear()
        {
            _summaryWriters.Clear();
            return this;
        }

        public IEnumerator<IReportWriter> GetEnumerator()
        {
            return _summaryWriters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
