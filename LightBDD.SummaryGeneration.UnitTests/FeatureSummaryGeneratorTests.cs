using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Execution.Results;
using LightBDD.UnitTests.Helpers;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Mocks = LightBDD.UnitTests.Helpers.Mocks;

namespace LightBDD.SummaryGeneration.UnitTests
{
    [TestFixture]
    public class FeatureSummaryGeneratorTests
    {
        [Test]
        public void SummaryGenerator_should_be_thread_safe()
        {
            var writer = Mock.Of<ISummaryWriter>();
            var generator = new FeatureSummaryGenerator(writer);
            var fixture = MockFixture.CreateNew();

            var mocks = fixture.CreateMany<Mocks.TestFeatureResult>(50).ToArray();
            var allMocks = new List<IFeatureResult>();
            for (int i = 0; i < 100; ++i)
                allMocks.AddRange(mocks);

            allMocks
            .AsParallel()
            .ForAll(generator.Aggregate);

            generator.Dispose();
            Mock.Get(writer).Verify(w => w.Save(It.Is<IFeatureResult[]>(r => r.Length == allMocks.Count)));
        }

        [Test]
        public void It_should_aggregate_all_results_and_save_them_on_dispose()
        {
            var summaryWriters = new[]
            {
                Mock.Of<ISummaryWriter>(),
                Mock.Of<ISummaryWriter>()
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
                Mock.Get(summaryWriter).Verify(w => w.Save(It.IsAny<IFeatureResult[]>()),Times.Never);

            generator.Dispose();

            foreach (var summaryWriter in summaryWriters)
                Mock.Get(summaryWriter).Verify(w => w.Save(It.Is<IFeatureResult[]>(f => f.SequenceEqual(results.OrderBy(r => r.Info.Name.ToString()).ToArray()))));
        }
    }
}