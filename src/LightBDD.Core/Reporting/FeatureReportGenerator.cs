using System;
using System.Collections.Concurrent;
using System.Linq;
using LightBDD.Core.Execution.Coordination;
using LightBDD.Core.Results;

namespace LightBDD.Core.Reporting
{
    /// <summary>
    /// Class allowing to generate and save reports for executed features.
    /// It supports multiple <see cref="IReportWriter"/> instances that can be specified in constructor.
    /// </summary>
    public class FeatureReportGenerator : IFeatureAggregator
    {
        private readonly IReportWriter[] _writers;
        private readonly ConcurrentQueue<IFeatureResult> _results = new ConcurrentQueue<IFeatureResult>();
        private bool _disposed;
        /// <summary>
        /// Constructor configuring report generator with <paramref name="writers"/> that would be used to write reports on generator disposal.
        /// </summary>
        /// <param name="writers"></param>
        public FeatureReportGenerator(params IReportWriter[] writers)
        {
            _writers = writers;
        }

        /// <summary>
        /// Aggregates given feature result.
        /// </summary>
        /// <param name="featureResult">Feature result to aggregate.</param>
        public void Aggregate(IFeatureResult featureResult)
        {
            _results.Enqueue(featureResult);
        }

        /// <summary>
        /// Writes all aggregated results and disposes the object.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;

            var results = _results.OrderBy(r => r.Info.Name.ToString(), StringComparer.OrdinalIgnoreCase).ToArray();
            foreach (var writer in _writers)
                writer.Save(results);
        }
    }
}