using System;
using System.Collections;
using System.Collections.Generic;
using LightBDD.Core.Reporting;

namespace LightBDD.Core.Configuration
{
    /// <summary>
    /// Configuration class allowing to customize report writers.
    /// </summary>
    public class ReportWritersConfiguration : FeatureConfiguration, IEnumerable<IReportWriter>
    {
        private readonly List<IReportWriter> _writers = new List<IReportWriter>();

        /// <summary>
        /// Adds <paramref name="writer"/> to report writers collection.
        /// </summary>
        /// <param name="writer">Writer to add.</param>
        /// <returns>Self.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="writer"/> is <c>null</c>.</exception>
        public ReportWritersConfiguration Add(IReportWriter writer)
        {
            ThrowIfSealed();
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            _writers.Add(writer);
            return this;
        }

        /// <summary>
        /// Removes specified, previously configured writer.
        /// </summary>
        /// <param name="writer">Writer instance to remove.</param>
        /// <returns>Self.</returns>
        public ReportWritersConfiguration Remove(IReportWriter writer)
        {
            ThrowIfSealed();
            _writers.Remove(writer);
            return this;
        }

        /// <summary>
        /// Removes all previously configured report writers.
        /// </summary>
        /// <returns>Self.</returns>
        public ReportWritersConfiguration Clear()
        {
            ThrowIfSealed();
            _writers.Clear();
            return this;
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<IReportWriter> GetEnumerator()
        {
            return _writers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
