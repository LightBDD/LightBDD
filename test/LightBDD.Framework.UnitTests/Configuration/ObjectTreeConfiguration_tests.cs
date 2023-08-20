using LightBDD.Core.Configuration;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Parameters.ObjectTrees;
using NUnit.Framework;
using Shouldly;
using System;
using System.Threading.Tasks;
using LightBDD.Core.Results;
using LightBDD.Framework.UnitTests.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace LightBDD.Framework.UnitTests.Configuration
{
    [TestFixture]
    public class ObjectTreeConfiguration_tests
    {
        [Test]
        public void It_should_return_default_builder()
        {
            new ObjectTreeConfiguration().Options.ShouldBeSameAs(ObjectTreeBuilderOptions.Default);
        }

        [Test]
        public void It_should_configure_builder()
        {
            var options = ObjectTreeBuilderOptions.Default.ClearMappers();
            var actual = new ObjectTreeConfiguration().UpdateOptions(options).Options;
            actual.ShouldBeSameAs(options);
        }

        [Test]
        public void It_should_not_allow_configuring_builder_when_sealed()
        {
            var cfg = new LightBddConfiguration();
            cfg.Seal();
            Assert.Throws<InvalidOperationException>(() => cfg.ForObjectTrees().UpdateOptions(ObjectTreeBuilderOptions.Default));
        }

        [Test]
        public async Task It_should_apply_configuration_on_GetConfigured_instance()
        {
            var options = ObjectTreeBuilderOptions.Default.WithMaxDepth(1);
            var capture = new OptionsCapture();

            void OnConfigure(LightBddConfiguration x)
            {
                x.ForObjectTrees().UpdateOptions(options);
                x.Services.AddSingleton(capture);
            }

            ObjectTreeBuilder.GetConfigured().Options.ShouldBeSameAs(ObjectTreeBuilderOptions.Default);

            var result = await TestableExecutionPipeline.Create(OnConfigure).ExecuteScenario<MyFixture>(f => f.MyScenario());
            result.Status.ShouldBe(ExecutionStatus.Passed);

            ObjectTreeBuilder.GetConfigured().Options.ShouldBeSameAs(ObjectTreeBuilderOptions.Default);
            capture.Options.ShouldBeSameAs(options);
        }

        class MyFixture
        {
            private readonly OptionsCapture _capture;

            public MyFixture(OptionsCapture capture)
            {
                _capture = capture;
            }

            [TestScenario]
            public Task MyScenario()
            {
                _capture.Options = ObjectTreeBuilder.GetConfigured().Options;
                return Task.CompletedTask;
            }
        }

        class OptionsCapture
        {
            public ObjectTreeBuilderOptions Options { get; set; }
        }
    }
}
