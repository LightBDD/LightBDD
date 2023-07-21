using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace LightBDD.Runner.IntegrationTests.Helpers
{
    class LightBddFirstOrderer : ITestCollectionOrderer
    {
        public IEnumerable<ITestCollection> OrderTestCollections(IEnumerable<ITestCollection> testCollections)
        {
            return testCollections.OrderByDescending(x => x.DisplayName == "LightBDD Test Execution Collection");
        }
    }
}