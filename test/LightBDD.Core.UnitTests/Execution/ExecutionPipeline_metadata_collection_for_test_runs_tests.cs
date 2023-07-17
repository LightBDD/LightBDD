using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Execution
{
    [TestFixture]
    public class ExecutionPipeline_metadata_collection_for_test_runs_tests
    {
        class PassingFeature
        {
            [TestScenario]
            public void MyScenario1()
            {
            }
        }

        class FailingFeature
        {
            [TestScenario]
            public void MyScenario2()
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public async Task It_should_collect_test_run_results()
        {
            var testRun = await TestableCoreExecutionPipeline.Default.Execute(
                typeof(PassingFeature), typeof(FailingFeature));

            testRun.Features.ShouldNotBeEmpty();

            testRun.ExecutionTime.ShouldNotBeNull();

            testRun.Info.Name.ToString().ShouldBe("LightBDD.Core.UnitTests");
            testRun.Info.TestSuite.ShouldBeEquivalentTo(TestSuite.Create(GetType().Assembly));
            testRun.Info.Name.ToString().ShouldBe("LightBDD.Core.UnitTests");
            testRun.Info.LightBddAssemblies.ShouldBeEquivalentTo(new[] { typeof(CoreMetadataProvider).Assembly }.Select(AssemblyInfo.From).ToArray());
        }

        [Test]
        public async Task It_should_fail_test_run_for_failing_scenarios()
        {
            var testRun = await TestableCoreExecutionPipeline.Default.Execute(
                typeof(PassingFeature), typeof(FailingFeature));

            testRun.OverallStatus.ShouldBe(ExecutionStatus.Failed);
            testRun.Features.Count.ShouldBe(2);
        }

        [Test]
        public async Task It_should_pass_test_run_for_passing_scenarios()
        {
            var testRun = await TestableCoreExecutionPipeline.Default.Execute(typeof(PassingFeature));

            testRun.OverallStatus.ShouldBe(ExecutionStatus.Passed);
            testRun.Features.Count.ShouldBe(1);
        }
    }
}
