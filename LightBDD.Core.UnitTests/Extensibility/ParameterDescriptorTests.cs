using System;
using LightBDD.Core.Extensibility;
using LightBDD.Core.UnitTests.Helpers;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Extensibility
{
    [TestFixture]
    public class ParameterDescriptorTests
    {
        [Test]
        public void FromConstant_should_allow_creating_constant()
        {
            var descriptor = ParameterDescriptor.FromConstant(ParameterInfoHelper.IntParameterInfo, 55);
            Assert.That(descriptor.IsConstant, Is.True);
            Assert.That(descriptor.ParameterInfo, Is.SameAs(ParameterInfoHelper.IntParameterInfo));
            Assert.That(descriptor.RawName, Is.EqualTo(ParameterInfoHelper.IntParameterInfo.Name));
            Assert.That(descriptor.ValueEvaluator, Is.Not.Null);
            Assert.That(descriptor.ValueEvaluator(null), Is.EqualTo(55));
        }

        [Test]
        public void FromInvocation_should_allow_creating_mutable_parameter()
        {
            var descriptor = ParameterDescriptor.FromInvocation(ParameterInfoHelper.IntParameterInfo, i => ((Incrementer)i).GetNext());
            Assert.That(descriptor.IsConstant, Is.False);
            Assert.That(descriptor.ParameterInfo, Is.SameAs(ParameterInfoHelper.IntParameterInfo));
            Assert.That(descriptor.RawName, Is.EqualTo(ParameterInfoHelper.IntParameterInfo.Name));
            Assert.That(descriptor.ValueEvaluator, Is.Not.Null);

            var incrementer = new Incrementer();
            Assert.That(descriptor.ValueEvaluator(incrementer), Is.EqualTo(1));
            Assert.That(descriptor.ValueEvaluator(incrementer), Is.EqualTo(2));
        }

        [Test]
        public void FromConstant_should_throw_if_parameterInfo_is_null()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => ParameterDescriptor.FromConstant(null, 0));
            Assert.That(ex.ParamName, Is.EqualTo("parameterInfo"));
        }

        [Test]
        public void FromInvocation_should_throw_if_parameterInfo_is_null()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => ParameterDescriptor.FromInvocation(null, o => 5));
            Assert.That(ex.ParamName, Is.EqualTo("parameterInfo"));
        }

        [Test]
        public void FromInvocation_should_throw_if_valueEvaluator_is_null()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => ParameterDescriptor.FromInvocation(ParameterInfoHelper.IntParameterInfo, null));
            Assert.That(ex.ParamName, Is.EqualTo("valueEvaluator"));
        }

        class Incrementer
        {
            private int _next;
            public int GetNext() => ++_next;
        }
    }
}