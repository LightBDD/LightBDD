using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Configuration;
using LightBDD.Core.Formatting.Values;
using Moq;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Configuration
{
    [TestFixture]
    public class ValueFormattingConfiguration_tests
    {
        [Test]
        public void It_should_initialize_object_with_default_values()
        {
            var configuration = new ValueFormattingConfiguration();
            Assert.That(configuration.GeneralFormatters, Is.Empty);
            Assert.That(configuration.StrictFormatters.Keys, Is.EquivalentTo(new[] { typeof(string) }));
        }

        [Test]
        public void It_should_not_allow_registering_null_general_formatter()
        {
            Assert.Throws<ArgumentNullException>(() => new ValueFormattingConfiguration().Register(null));
        }

        [Test]
        public void It_should_not_allow_registering_null_strict_formatter()
        {
            Assert.Throws<ArgumentNullException>(() => new ValueFormattingConfiguration().Register(null, Mock.Of<IValueFormatter>()));
            Assert.Throws<ArgumentNullException>(() => new ValueFormattingConfiguration().Register(typeof(object), null));
        }

        [Test]
        public void It_should_register_general_formatter()
        {
            var formatter1 = Mock.Of<IConditionalValueFormatter>();
            var formatter2 = Mock.Of<IConditionalValueFormatter>();
            var configuration = new ValueFormattingConfiguration()
                .Register(formatter1)
                .Register(formatter2);

            Assert.That(configuration.GeneralFormatters.ToArray(), Is.EqualTo(new[] { formatter1, formatter2 }));
        }

        [Test]
        public void It_should_register_strict_formatter_allowing_to_override_previous_ones()
        {
            var formatter1 = Mock.Of<IValueFormatter>();
            var formatter2 = Mock.Of<IValueFormatter>();
            var configuration = new ValueFormattingConfiguration()
                .Register(typeof(string), formatter1)
                .Register(typeof(int), formatter1)
                .Register(typeof(object), formatter1)
                .Register(typeof(object), formatter2);

            Assert.That(
                configuration.StrictFormatters.ToDictionary(x => x.Key, x => x.Value),
                Is.EqualTo(new Dictionary<Type, IValueFormatter>
                {
                    {typeof(string), formatter1},
                    {typeof(int), formatter1},
                    {typeof(object), formatter2}
                }));
        }

        [Test]
        public void Clear_should_clear_all_formatters()
        {
            var configuration = new ValueFormattingConfiguration()
                .Register(Mock.Of<IConditionalValueFormatter>())
                .Register(typeof(char), Mock.Of<IValueFormatter>())
                .Clear();

            Assert.That(configuration.GeneralFormatters, Is.Empty);
            Assert.That(configuration.StrictFormatters, Is.Empty);
        }

        [Test]
        public void Configuration_should_be_sealable()
        {
            var lighbddConfig = new LightBddConfiguration();
            var cfg = lighbddConfig.Get<ValueFormattingConfiguration>();

            var generalFormatters = cfg.GeneralFormatters.ToArray();
            var strictFormatters = cfg.StrictFormatters.ToArray();

            lighbddConfig.Seal();

            Assert.Throws<InvalidOperationException>(() => cfg.Clear());
            Assert.Throws<InvalidOperationException>(() => cfg.Register(Mock.Of<IConditionalValueFormatter>()));
            Assert.Throws<InvalidOperationException>(() => cfg.Register(typeof(object), Mock.Of<IValueFormatter>()));

            Assert.That(cfg.GeneralFormatters, Is.EquivalentTo(generalFormatters));
            Assert.That(cfg.StrictFormatters, Is.EquivalentTo(strictFormatters));
        }
    }
}