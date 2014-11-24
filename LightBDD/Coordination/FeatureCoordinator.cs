using System;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using LightBDD.Formatters;
using LightBDD.Results;

namespace LightBDD.Coordination
{
    /// <summary>
    /// Feature coordinator singleton class allowing to collect feature results, pass them to specified aggregator and notify it when all tests finished.
    /// It allows to customize aggregator - by default it is FeatureSummaryAggregator that saves feature results to XML.
    /// This class guarantees aggregator notification on AppDomain unload, but offers also method to notify aggregator manually.
    /// </summary>
    [DebuggerStepThrough]
    public class FeatureCoordinator : CriticalFinalizerObject
    {
        private static readonly FeatureCoordinator _instance = new FeatureCoordinator(SummaryGeneratorFactory.Create());
        private bool _finished;

        /// <summary>
        /// Event emitted just before Aggregator.Finished() method is called;
        /// </summary>
        public event Action OnBeforeFinish;
        /// <summary>
        /// Event emitted just after Aggregator.Finished() method is called;
        /// </summary>
        public event Action OnAfterFinish;

        /// <summary>
        /// Coordinator instance.
        /// </summary>
        public static FeatureCoordinator Instance { get { return _instance; } }

        /// <summary>
        /// Aggregator used to collect feature results.
        /// 
        /// By default, SummaryGenerator is used, which configuration is read from app.config file.
        /// The following code presents how default SummaryGenerator can be configured:
        /// <code>
        /// &lt;?xml version="1.0" encoding="utf-8" ?&gt;
        ///&lt;configuration&gt;
        ///  &lt;configSections&gt;
        ///    &lt;section name="lightbdd" type="LightBDD.Configuration.LightBDDConfiguration, LightBDD"/&gt;
        ///  &lt;/configSections&gt;
        ///  &lt;lightbdd&gt;
        ///    &lt;summaryWriters&gt;
        ///      &lt;!-- FeatureSummary.xml is added by default. Use &lt;clear /&gt; to remove it.--&gt;
        ///      &lt;add formatter="LightBDD.Results.Formatters.PlainTextResultFormatter, LightBDD" output="FeatureSummary.txt"/&gt;
        ///      &lt;add formatter="LightBDD.Results.Formatters.HtmlResultFormatter, LightBDD" output="FeatureSummary.html"/&gt;
        ///    &lt;/summaryWriters&gt;
        ///  &lt;/lightbdd&gt;
        ///&lt;/configuration&gt;
        /// </code>
        /// 
        /// It is also possible to customize aggregator by redefining it's value - the best time to set it is before any tests run (like in class with [SetUpFixture] attribute).
        /// </summary>
        public IFeatureAggregator Aggregator { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="aggregator">Aggregator to use.</param>
        protected FeatureCoordinator(IFeatureAggregator aggregator)
        {
            Aggregator = aggregator;
        }

        /// <summary>
        /// Adds feature to aggregator.
        /// </summary>
        /// <param name="feature">Feature to aggregate.</param>
        public void AddFeature(IFeatureResult feature)
        {
            VerifyFinishedState();
            Aggregator.AddFeature(feature);
        }

        /// <summary>
        /// Notifies aggregator that all features has been already added.
        /// PLEASE NOTE that this method does not have to be normally called - FeatureCoordinator will notify aggregator on AppDomain unload anyway.
        /// </summary>
        public void Finished()
        {
            VerifyFinishedState();
            try
            {
                if (OnBeforeFinish != null)
                    OnBeforeFinish();
                Aggregator.Finished();
                if (OnAfterFinish != null)
                    OnAfterFinish();
            }
            finally
            {
                _finished = true;
            }
        }

        private void VerifyFinishedState()
        {
            if (_finished)
                throw new ObjectDisposedException("FeatureCoordinator work is already finished.");
        }

        /// <summary>
        /// Notifies aggregator that all features has been already added.
        /// </summary>
        ~FeatureCoordinator()
        {
            try
            {
                if (_finished)
                    return;

                var sw = new Stopwatch();
                sw.Start();
                Finished();
                sw.Stop();
                Trace.TraceInformation("Summary aggregation took {0}", sw.Elapsed.FormatPretty());
                if (sw.Elapsed > TimeSpan.FromMilliseconds(1500))
                    Trace.TraceWarning("WARNING: Please consider to call FeatureCoordinator.Instance.Finished() manually, otherwise if execution time reach 2 seconds, .NET framework will kill aggregation process.");
            }
            catch (Exception e)
            {
                Trace.TraceError("Summary aggregation failed: {0}", e);
            }
        }
    }
}
