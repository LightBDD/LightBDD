using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using LightBDD.Coordination;
using LightBDD.Results;

namespace LightBDD.SummaryGeneration
{
    /// <summary>
    /// SummaryGenerator class allowing to generate and save summary for executed feature scenarios.
    /// It supports multiple summary writers that can be specified in constructor.
    /// </summary>
    public class SummaryGenerator : IFeatureAggregator
    {
        private readonly ISummaryWriter[] _summaryWriters;
        private readonly List<IFeatureResult> _results = new List<IFeatureResult>();

        /// <summary>
        /// Class constructor allowing to specify summary writers that would be used to save feature summary.
        /// </summary>
        /// <param name="summaryWriters"></param>
        public SummaryGenerator(params ISummaryWriter[] summaryWriters)
        {
            _summaryWriters = summaryWriters ?? new ISummaryWriter[0];
        }

        /// <summary>
        /// Adds given feature to summary.
        /// </summary>
        /// <param name="feature">Feature result.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddFeature(IFeatureResult feature)
        {
            _results.Add(feature);
        }

        /// <summary>
        /// Saves all aggregated feature results by using summary writers specified in constructor.
        /// </summary>
        public void Finished()
        {
            var results = GetResults();
            foreach (var output in _summaryWriters)
                output.Save(results);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private IFeatureResult[] GetResults()
        {
            return _results.OrderBy(r => r.Name).ToArray();
        }
    }
}