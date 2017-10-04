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
            Assert.That(configuration.ExplicitFormatters.Keys, Is.EquivalentTo(new[] { typeof(string) }));
        }

        [Test]
        public void It_should_not_allow_registering_null_general_formatter()
        {
            Assert.Throws<ArgumentNullException>(() => new ValueFormattingConfiguration().RegisterGeneral(null));
        }

        [Test]
        public void It_should_not_allow_registering_null_explicit_formatter()
        {
            Assert.Throws<ArgumentNullException>(() => new ValueFormattingConfiguration().RegisterExplicit(null, Mock.Of<IValueFormatter>()));
            Assert.Throws<ArgumentNullException>(() => new ValueFormattingConfiguration().RegisterExplicit(typeof(object), null));
        }

        [Test]
        public void It_should_register_general_formatter()
        {
            var formatter1 = Mock.Of<IConditionalValueFormatter>();
            var formatter2 = Mock.Of<IConditionalValueFormatter>();
            var configuration = new ValueFormattingConfiguration()
                .RegisterGeneral(formatter1)
                .RegisterGeneral(formatter2);

            Assert.That(configuration.GeneralFormatters.ToArray(), Is.EqualTo(new[] { formatter1, formatter2 }));
        }

        [Test]
        public void It_should_register_explicit_formatter_allowing_to_override_previous_ones()
        {
            var formatter1 = Mock.Of<IValueFormatter>();
            var formatter2 = Mock.Of<IValueFormatter>();
            var configuration = new ValueFormattingConfiguration()
                .RegisterExplicit(typeof(string), formatter1)
                .RegisterExplicit(typeof(int), formatter1)
                .RegisterExplicit(typeof(object), formatter1)
                .RegisterExplicit(typeof(object), formatter2);

            Assert.That(
                configuration.ExplicitFormatters.ToDictionary(x => x.Key, x => x.Value),
                Is.EqualTo(new Dictionary<Type, IValueFormatter>
                {
                    {typeof(string), formatter1},
                    {typeof(int), formatter1},
                    {typeof(object), formatter2}
                }));
        }

        [Test]
        public void It_should_clear_all_general_formatters()
        {
            var configuration = new ValueFormattingConfiguration()
                .RegisterGeneral(Mock.Of<IConditionalValueFormatter>())
                .ClearGeneral();

            Assert.That(configuration.GeneralFormatters, Is.Empty);
        }

        [Test]
        public void It_should_clear_all_explicit_formatters()
        {
            var configuration = new ValueFormattingConfiguration()
                .RegisterExplicit(typeof(char), Mock.Of<IValueFormatter>())
                .ClearExplicit();

            Assert.That(configuration.ExplicitFormatters, Is.Empty);
        }

        [Test]
        public void Configuration_should_be_sealable()
        {
            var lighbddConfig = new LightBddConfiguration();
            var cfg = lighbddConfig.Get<ValueFormattingConfiguration>();

            var generalFormatters = cfg.GeneralFormatters.ToArray();
            var strictFormatters = cfg.ExplicitFormatters.ToArray();

            lighbddConfig.Seal();

            Assert.Throws<InvalidOperationException>(() => cfg.ClearGeneral());
            Assert.Throws<InvalidOperationException>(() => cfg.ClearExplicit());
            Assert.Throws<InvalidOperationException>(() => cfg.RegisterGeneral(Mock.Of<IConditionalValueFormatter>()));
            Assert.Throws<InvalidOperationException>(() => cfg.RegisterExplicit(typeof(object), Mock.Of<IValueFormatter>()));

            Assert.That(cfg.GeneralFormatters, Is.EquivalentTo(generalFormatters));
            Assert.That(cfg.ExplicitFormatters, Is.EquivalentTo(strictFormatters));
        }
    }
}