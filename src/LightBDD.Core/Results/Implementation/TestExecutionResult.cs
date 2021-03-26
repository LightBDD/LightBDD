using System.Collections.Generic;
using System.Reflection;

namespace LightBDD.Core.Results.Implementation
{
    internal class TestExecutionResult : ITestExecutionResult
    {
        private readonly IReadOnlyList<IFeatureResult> _features;

        public TestExecutionResult(ExecutionTime executionTime, IReadOnlyList<IFeatureResult> features)
        {
            ExecutionTime = executionTime;
            _features = features;
        }

        public ExecutionTime ExecutionTime { get; }
        public IEnumerable<IFeatureResult> GetFeatures() => _features;
    }
}