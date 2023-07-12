using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Extensibility.Results;
using LightBDD.Core.Metadata;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Framework;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Extensibility
{
    [TestFixture]
    public class CoreMetadataProvider_tests
    {
        private CoreMetadataProvider _metadataProvider;

        [SetUp]
        public void SetUp()
        {
            _metadataProvider = new TestMetadataProvider();
        }

        [Test]
        public void It_should_throw_if_nameFormatter_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new TestMetadataProvider((LightBddConfiguration)null));
        }

        [Test]
        public void GetFeatureInfo_should_capture_feature_information_from_type()
        {
            var featureInfo = _metadataProvider.GetFeatureInfo(typeof(Feature_type));
            Assert.That(featureInfo.Name.ToString(), Is.EqualTo("Feature type"));
            Assert.That(featureInfo.Name.Parameters, Is.Empty);
            Assert.That(featureInfo.Description, Is.EqualTo("description"));
            Assert.That(featureInfo.Labels, Is.EqualTo(new[] { "l1", "l2" }));
        }

        [Test]
        public void GetScenarioName_should_capture_parameterless_scenario_name_from_descriptor()
        {
            var method = typeof(Feature_type).GetMethod(nameof(Feature_type.Some_method_without_arguments));
            var scenarioName = _metadataProvider.GetScenarioName(new ScenarioDescriptor(method, Array.Empty<object>()));

            Assert.That(scenarioName.ToString(), Is.EqualTo("Some method without arguments"));
            Assert.That(scenarioName.Parameters, Is.Empty);
        }

        [Test]
        public void GetScenarioName_should_capture_parameterized_scenario_name_from_descriptor_allowing_unknown_arguments()
        {
            var method = typeof(Feature_type).GetMethod(nameof(Feature_type.Some_method_with_argument_arg1_and_arg2));
            var scenarioName = _metadataProvider.GetScenarioName(new ScenarioDescriptor(method, null));

            Assert.That(scenarioName.ToString(), Is.EqualTo("Some method with argument arg1 and arg2"));
            Assert.That(scenarioName.Parameters, Is.Empty);
        }

        [Test]
        public void GetScenarioLabels_should_capture_scenario_labels_from_method()
        {
            var method = typeof(Feature_type).GetMethod(nameof(Feature_type.Some_method));
            Assert.That(_metadataProvider.GetScenarioLabels(method), Is.EqualTo(new[] { "s1", "s2" }));
        }

        [Test]
        public void GetScenarioCategories_should_capture_scenario_categories_from_method()
        {
            var method = typeof(Feature_type).GetMethod(nameof(Feature_type.Some_method));
            Assert.That(_metadataProvider.GetScenarioCategories(method), Is.EqualTo(new[] { "c1", "c2" }));
        }

        [Test]
        public void GetStepName_should_capture_name_from_step_descriptor_but_leave_parameters_unknown()
        {
            var descriptor = new StepDescriptor(
                ParameterInfoHelper.GetMethodInfo<int>(new Feature_type().Some_step_with_argument),
                (o, a) => Task.FromResult(DefaultStepResultDescriptor.Instance),
                ParameterDescriptor.FromConstant(
                    ParameterInfoHelper.GetMethodParameter<int>(new Feature_type().Some_step_with_argument), 5))
            {
                PredefinedStepType = "given"
            };

            var stepName = _metadataProvider.GetStepName(descriptor, null);
            Assert.That(stepName.StepTypeName.ToString(), Is.EqualTo("GIVEN"));
            Assert.That(stepName.NameFormat, Is.EqualTo("Some step with argument \"{0}\""));
            Assert.That(stepName.ToString(), Is.EqualTo("GIVEN Some step with argument \"<?>\""));
        }

        [Test]
        [TestCase(true, "raw_name", "raw name")]
        [TestCase(false, "raw_name", "raw_name")]
        public void GetStepName_should_honor_IsNameFormattingRequired_flag(bool flag, string rawName, string expectedName)
        {
            var descriptor =
                new StepDescriptor(rawName, (o, a) => Task.FromResult(DefaultStepResultDescriptor.Instance))
                {
                    IsNameFormattingRequired = flag
                };
            var stepName = _metadataProvider.GetStepName(descriptor, null);
            Assert.That(stepName.NameFormat, Is.EqualTo(expectedName));
        }

        [Test]
        public void GetStepDecorators_should_return_extensions_in_order_if_declaring_type_does_not_have_extensions()
        {
            Action<int> step = new Feature_type().Some_step_with_argument;
            var extensions = _metadataProvider.GetStepDecorators(new StepDescriptor(step.GetMethodInfo(), (o, a) => Task.FromResult(DefaultStepResultDescriptor.Instance)));
            var expectedOrder = new[] { 0, 2, 3, 5 };

            Assert.That(extensions.Cast<IStepDecoratorAttribute>().Select(x => x.Order).ToArray(),
                Is.EqualTo(expectedOrder));
        }

        [Test]
        public void GetStepDecorators_should_return_extensions_in_order_favoring_declaring_type_extensions()
        {
            Action<int> step = new Feature_with_class_decorators().Some_step_with_argument;
            var extensions = _metadataProvider.GetStepDecorators(new StepDescriptor(step.GetMethodInfo(), (o, a) => Task.FromResult(DefaultStepResultDescriptor.Instance)));
            var expectedOrder = new[] { 3, 4, 6, 7, 8 };

            Assert.That(extensions.Cast<IStepDecoratorAttribute>().Select(x => x.Order).ToArray(),
                Is.EqualTo(expectedOrder));
        }

        [Test]
        public void GetStepDecorators_should_return_empty_collection_if_method_and_declaring_type_does_not_have_extensions()
        {
            Action<int, string> step = new Feature_type().Some_method_with_argument_arg1_and_arg2;
            var extensions = _metadataProvider.GetStepDecorators(new StepDescriptor(step.GetMethodInfo(), (o, a) => Task.FromResult(DefaultStepResultDescriptor.Instance)));
            Assert.That(extensions, Is.Empty);
        }

        [Test]
        public void GetStepDecorators_should_return_empty_collection_if_descriptor_has_null_MethodInfo()
        {
            var extensions = _metadataProvider.GetStepDecorators(new StepDescriptor("abc123", (o, a) => Task.FromResult(DefaultStepResultDescriptor.Instance)));
            Assert.That(extensions, Is.Empty);
        }

        [Test]
        public void GetScenarioDecorators_should_return_extensions_in_order_if_declaring_type_does_not_have_extensions()
        {
            Action<int> step = new Feature_type().Some_method;
            var extensions = _metadataProvider.GetScenarioDecorators(new ScenarioDescriptor(step.GetMethodInfo(), null));
            var expectedOrder = new[] { -5, -3, -2, 0 };

            Assert.That(extensions.Cast<IScenarioDecoratorAttribute>().Select(x => x.Order).ToArray(),
                Is.EqualTo(expectedOrder));
        }

        [Test]
        public void GetScenarioDecorators_should_return_extensions_in_order_favoring_declaring_type_extensions()
        {
            Action<int> step = new Feature_with_class_decorators().Some_method;
            var extensions = _metadataProvider.GetScenarioDecorators(new ScenarioDescriptor(step.GetMethodInfo(), null));
            var expectedOrder = new[] { 1, 2, -5, -3, -2 };

            Assert.That(extensions.Cast<IScenarioDecoratorAttribute>().Select(x => x.Order).ToArray(),
                Is.EqualTo(expectedOrder));
        }


        [Test]
        public void GetScenarioDecorators_should_return_empty_collection_if_method_and_declaring_type_does_not_have_extensions()
        {
            Action step = new Feature_type().Some_method_without_arguments;
            var extensions = _metadataProvider.GetScenarioDecorators(new ScenarioDescriptor(step.GetMethodInfo(), null));
            Assert.That(extensions, Is.Empty);
        }

        [Test]
        public void GetTestRunInfo_should_provide_information_about_test_run()
        {
            var info = _metadataProvider.GetTestRunInfo();
            info.TestSuite.ShouldBeEquivalentTo(TestSuite.Create(GetType().Assembly));
            info.Name.ToString().ShouldBe("LightBDD.Core.UnitTests");

            var expected = new[] { typeof(TestMetadataProvider).Assembly, typeof(CoreMetadataProvider).Assembly }.Select(AssemblyInfo.From).ToArray();
            info.LightBddAssemblies.ShouldBeEquivalentTo(expected);
        }

        [FeatureDescription("description")]
        [Label("l1")]
        [Label("l2")]
        private class Feature_type
        {
            [Label("s1")]
            [Label("s2")]
            [ScenarioCategory("c1")]
            [ScenarioCategory("c2")]
            [MyScenarioDecorator(Order = -3)]
            [MyScenarioDecorator(Order = -2)]
            [MyScenarioDecorator(Order = -5)]
            [MyScenarioDecorator]
            public void Some_method(int argument) { }

            public void Some_method_without_arguments() { }
            public void Some_method_with_argument_arg1_and_arg2(int arg1, string arg2) { }

            [MyStepDecorator]
            [MyStepDecorator(Order = 3)]
            [MyStepDecorator(Order = 2)]
            [MyStepDecorator(Order = 5)]
            public void Some_step_with_argument(int argument) { }
        }

        [MyScenarioClassDecorator(Order = 2)]
        [MyScenarioClassDecorator(Order = 1)]
        [MyStepClassDecorator(Order = 4)]
        [MyStepClassDecorator(Order = 3)]
        private class Feature_with_class_decorators
        {
            [MyScenarioDecorator(Order = -3)]
            [MyScenarioDecorator(Order = -2)]
            [MyScenarioDecorator(Order = -5)]
            public void Some_method(int argument) { }

            public void Some_method_without_arguments() { }
            public void Some_method_with_argument_arg1_and_arg2(int arg1, string arg2) { }

            [MyStepDecorator(Order = 7)]
            [MyStepDecorator(Order = 6)]
            [MyStepDecorator(Order = 8)]
            public void Some_step_with_argument(int argument) { }
        }

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        private class MyStepDecoratorAttribute : Attribute, IStepDecoratorAttribute
        {
            public Task ExecuteAsync(IStep step, Func<Task> stepInvocation)
            {
                throw new NotImplementedException();
            }

            public int Order { get; set; }
        }
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        private class MyScenarioDecorator : Attribute, IScenarioDecoratorAttribute
        {
            public Task ExecuteAsync(IScenario scenario, Func<Task> scenarioInvocation)
            {
                throw new NotImplementedException();
            }

            public int Order { get; set; }
        }
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
        private class MyScenarioClassDecorator : Attribute, IScenarioDecoratorAttribute
        {
            public Task ExecuteAsync(IScenario scenario, Func<Task> scenarioInvocation)
            {
                throw new NotImplementedException();
            }

            public int Order { get; set; }
        }
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
        private class MyStepClassDecorator : Attribute, IStepDecoratorAttribute
        {
            public Task ExecuteAsync(IStep step, Func<Task> stepInvocation)
            {
                throw new NotImplementedException();
            }

            public int Order { get; set; }
        }
    }
}