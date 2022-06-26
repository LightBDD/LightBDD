using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Reporting
{
    [TestFixture]
    public class StepExecution_tests
    {
        #region Setup/Teardown

        private IFeatureRunner _feature;
        private IBddRunner _runner;

        [SetUp]
        public void SetUp()
        {
            _feature = TestableFeatureRunnerRepository.GetRunner(GetType());
            _runner = _feature.GetBddRunner(this);
        }

        #endregion

        [Test]
        public async Task AttachFile_should_attach_file_to_step_results()
        {
            await _runner.Test().TestScenarioAsync(Step1_with_attachment, Step2_with_attachment);
            var steps = _feature.GetFeatureResult().GetScenarios().Single().GetSteps().ToArray();

            Assert.That(steps[0].FileAttachments.Select(a => $"{a.Name}|{a.FilePath}"), Is.EqualTo(new[] { "step1|path1" }));
            Assert.That(steps[1].FileAttachments.Select(a => $"{a.Name}|{a.FilePath}"), Is.EqualTo(new[] { "step2_1|path2", "step2_2|path3" }));
        }

        private async Task Step1_with_attachment()
        {
            await StepExecution.Current.AttachFile(_ => Task.FromResult(new FileAttachment("step1", "path1")));
        }

        private async Task Step2_with_attachment()
        {
            await StepExecution.Current.AttachFile(_ => Task.FromResult(new FileAttachment("step2_1", "path2")));
            await StepExecution.Current.AttachFile(_ => Task.FromResult(new FileAttachment("step2_2", "path3")));
        }
    }
}
