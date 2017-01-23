using System.Collections.Concurrent;
using System.Linq;
using LightBDD.Core.Execution.Coordination;
using LightBDD.Core.Results;

namespace LightBDD.Reporting
{
    public class FeatureSummaryGenerator : IFeatureAggregator
    {
        private readonly ISummaryWriter[] _writers;
        private readonly ConcurrentQueue<IFeatureResult> _results = new ConcurrentQueue<IFeatureResult>();
        private bool _disposed;

        public FeatureSummaryGenerator(params ISummaryWriter[] writers)
        {
            _writers = writers;
        }

        public void Aggregate(IFeatureResult featureResult)
        {
            _results.Enqueue(featureResult);
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;

            var results = _results.OrderBy(r => r.Info.Name.ToString()).ToArray();
            foreach (var writer in _writers)
                writer.Save(results);
        }
    }
}