using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Results;
using LightBDD.Core.Formatting.NameDecorators;
using LightBDD.Core.Metadata;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Metadata
{
    [TestFixture]
    public class StepNameInfo_tests
    {
        [Test]
        public void Format_should_decorate_step_name()
        {
            var stepName = CreateStepNameInfo("raw");
            var format = stepName.Format(new TestDecorator());
            Assert.That(format, Is.EqualTo("|raw|"));
        }

        [Test]
        public void Format_should_decorate_step_name_with_parameter()
        {
            var stepName = CreateStepNameInfo("raw {0}, {1}", 2);
            var format = stepName.Format(new TestDecorator());
            Assert.That(format, Is.EqualTo("|raw '<?>', '<?>'|"));
        }

        [Test]
        public void Format_should_decorate_step_name_with_step_type()
        {
            var stepName = CreateStepNameInfo("type", "raw");
            var format = stepName.Format(new TestDecorator());
            Assert.That(format, Is.EqualTo("<TYPE> |raw|"));
        }

        [Test]
        public void Format_should_decorate_step_name_with_step_type_and_parameters()
        {
            var stepName = CreateStepNameInfo("type", "raw {0}", 1);
            var format = stepName.Format(new TestDecorator());
            Assert.That(format, Is.EqualTo("<TYPE> |raw '<?>'|"));
        }

        [Test]
        public void Format_should_decorate_step_name_using_name_formatter()
        {
            var stepName = CreateStepNameInfo("type", "raw {0}", 1);
            var format = stepName.Format((INameDecorator)new TestDecorator());
            Assert.That(format, Is.EqualTo("|raw '<?>'|"));
        }

        private static IStepNameInfo CreateStepNameInfo(string name, int args = 0) => CreateStepNameInfo(null, name, args);
        private static IStepNameInfo CreateStepNameInfo(string type, string name, int args = 0)
        {
            Task<IStepResultDescriptor> Invocation(object ctx, object[] a) => Task.FromResult(DefaultStepResultDescriptor.Instance);
            void SomeFunction(int x) { }

            Action<int> fn = SomeFunction;
            var param = fn.GetMethodInfo().GetParameters().Single();


            var descriptor = new StepDescriptor(name, Invocation,
                Enumerable.Range(0, args).Select(a => ParameterDescriptor.FromConstant(param, a)).ToArray())
            {
                PredefinedStepType = type
            };
            return new TestMetadataProvider().GetStepName(descriptor, "");
        }

        class TestDecorator : IStepNameDecorator
        {
            public string DecorateParameterValue(INameParameterInfo parameter)
            {
                return $"'{parameter.FormattedValue}'";
            }

            public string DecorateNameFormat(string nameFormat)
            {
                return $"|{nameFormat}|";
            }

            public string DecorateStepTypeName(IStepTypeNameInfo stepTypeName)
            {
                return $"<{stepTypeName.Name}>";
            }
        }
    }
}
