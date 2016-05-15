using System.Linq;
using LightBDD.Core.Execution.Results;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.SummaryGeneration.UnitTests
{
    [TestFixture]
    public class FeatureSummaryGeneratorTests
    {
        [Test]
        public void It_should_aggregate_all_results_and_save_them_on_dispose()
        {
            var summaryWriters = new[]
            {
                MockRepository.GenerateMock<ISummaryWriter>(),
                MockRepository.GenerateMock<ISummaryWriter>()
            };
            var generator = new FeatureSummaryGenerator(summaryWriters);

            var results = new[]
            {
                Mocks.CreateFeatureResult("name2","desc","label1"),
                Mocks.CreateFeatureResult("name1","desc","label1"),
                Mocks.CreateFeatureResult("name4","desc","label1"),
                Mocks.CreateFeatureResult("name3","desc","label1"),
            };

            foreach (var result in results)
                generator.Aggregate(result);

            foreach (var summaryWriter in summaryWriters)
                summaryWriter.AssertWasNotCalled(w => w.Save(Arg<IFeatureResult[]>.Is.Anything));

            generator.Dispose();

            foreach (var summaryWriter in summaryWriters)
                summaryWriter.AssertWasCalled(w => w.Save(Arg<IFeatureResult[]>.List.Equal(results.OrderBy(r => r.Info.Name.ToString()).ToArray())));
        }
    }
}