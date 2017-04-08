using System;
using System.Linq;
using System.Reflection;
using LightBDD.Core.Extensibility;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Extensibility
{
    [TestFixture]
    public class ScenarioDescriptor_tests
    {

        [Test]
        public void It_should_not_accept_null_methods()
        {
            Assert.Throws<ArgumentNullException>(() => new ScenarioDescriptor(null, null));
        }
        [Test]
        public void It_should_accept_parameterless_methods()
        {
            var methodInfo = GetMethod(nameof(Parameterless_method));
            var descriptor = new ScenarioDescriptor(methodInfo, new object[0]);
            Assert.That(descriptor.MethodInfo, Is.SameAs(methodInfo));
            Assert.That(descriptor.Parameters, Is.Empty);
        }

        [Test]
        [TestCase(5, "abc")]
        [TestCase(5, "")]
        [TestCase(5, null)]
        public void It_should_accept_parameterized_methods(int a1, string a2)
        {
            var methodInfo = GetMethod(nameof(Parameterized_method));
            var descriptor = new ScenarioDescriptor(methodInfo, new object[] { a1, a2 });
            Assert.That(descriptor.MethodInfo, Is.SameAs(methodInfo));
            Assert.That(descriptor.Parameters, Is.Not.Empty);

            var parameters = descriptor.Parameters.ToArray();
            AssertParameter(parameters[0], "x", a1);
            AssertParameter(parameters[1], "y", a2);
        }

        [Test]
        public void It_should_accept_parameterized_methods_with_unknown_arguments()
        {
            var methodInfo = GetMethod(nameof(Parameterized_method));
            var descriptor = new ScenarioDescriptor(methodInfo, null);
            Assert.That(descriptor.MethodInfo, Is.SameAs(methodInfo));
            Assert.That(descriptor.Parameters, Is.Empty);
        }

        [Test]
        public void It_should_accept_parameterless_methods_with_unknown_arguments()
        {
            var methodInfo = GetMethod(nameof(Parameterless_method));
            var descriptor = new ScenarioDescriptor(methodInfo, null);
            Assert.That(descriptor.MethodInfo, Is.SameAs(methodInfo));
            Assert.That(descriptor.Parameters, Is.Empty);
        }

        [Test]
        public void It_should_not_accept_parameterless_methods_with_arguments()
        {
            var methodInfo = GetMethod(nameof(Parameterless_method));
            var ex = Assert.Throws<InvalidOperationException>(() => new ScenarioDescriptor(methodInfo, new object[] { "abc" }));
            Assert.That(ex.Message, Is.EqualTo("Provided method Void Parameterless_method() has different number of parameters than provided argument list: [abc]"));
        }

        [Test]
        public void It_should_not_accept_parameterized_methods_with_wrong_argument_count()
        {
            var methodInfo = GetMethod(nameof(Parameterized_method));
            var ex = Assert.Throws<InvalidOperationException>(() => new ScenarioDescriptor(methodInfo, new object[] { 4 }));
            Assert.That(ex.Message, Is.EqualTo("Provided method Void Parameterized_method(Int32, System.String) has different number of parameters than provided argument list: [4]"));
        }

        [Test]
        public void It_should_not_accept_parameterized_methods_with_wrong_argument_type()
        {
            var methodInfo = GetMethod(nameof(Parameterized_method));
            var ex = Assert.Throws<InvalidOperationException>(() => new ScenarioDescriptor(methodInfo, new object[] { 4, 5 }));
            Assert.That(ex.Message, Is.EqualTo("Provided argument System.Int32 '5' is not assignable to parameter index 1 of method Void Parameterized_method(Int32, System.String)"));
        }

        private void AssertParameter(ParameterDescriptor descriptor, string name, object value)
        {
            Assert.That(descriptor.IsConstant, Is.True);
            Assert.That(descriptor.RawName, Is.EqualTo(name));
            Assert.That(descriptor.ValueEvaluator(null), Is.EqualTo(value));
            Assert.That(descriptor.ParameterInfo.Name, Is.EqualTo(name));
        }

        private MethodInfo GetMethod(string methodName)
        {
            return GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private void Parameterless_method()
        {

        }

        private void Parameterized_method(int x, string y)
        {

        }
    }
}
