using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Framework;
using LightBDD.ScenarioHelpers;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Reporting
{
    [TestFixture]
    public class StepExecution_tests
    {
        [Test]
        public async Task AttachFile_should_attach_file_to_step_results()
        {
            var scenario = await TestableScenarioFactory.Default.RunScenario(r => r.Test().TestScenario(Step1_with_attachment, Step2_with_attachment));
            var steps = scenario.GetSteps().ToArray();

            Assert.That(steps[0].FileAttachments.Select(a => $"{a.Name}|{a.FilePath}|{a.RelativePath}"), Is.EqualTo(new[] { "step1|path1|file1" }));
            Assert.That(steps[1].FileAttachments.Select(a => $"{a.Name}|{a.FilePath}|{a.RelativePath}"), Is.EqualTo(new[] { "step2_1|path2|file2", "step2_2|path3|file3" }));
        }

        private async Task Step1_with_attachment()
        {
            await StepExecution.Current.AttachFile(_ => Task.FromResult(new FileAttachment("step1", "path1", "file1")));
        }

        private async Task Step2_with_attachment()
        {
            await StepExecution.Current.AttachFile(_ => Task.FromResult(new FileAttachment("step2_1", "path2", "file2")));
            await StepExecution.Current.AttachFile(_ => Task.FromResult(new FileAttachment("step2_2", "path3", "file3")));
        }
    }
}
