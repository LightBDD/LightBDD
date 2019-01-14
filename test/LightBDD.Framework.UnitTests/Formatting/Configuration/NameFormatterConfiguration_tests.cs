using LightBDD.Core.Configuration;
using LightBDD.Core.Formatting;
using Moq;
using NUnit.Framework;
using System;

namespace LightBDD.Framework.UnitTests.Formatting.Configuration
{
    [TestFixture]
    public class NameFormatterConfiguration_tests
    {
        [Test]
        public void It_should_throw_if_formatter_was_not_set()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => new NameFormatterConfiguration().GetFormatter());
            Assert.That(ex.Message, Is.EqualTo("INameFormatter was not specified."));
        }

        [Test]
        public void It_should_not_allow_null_formatter()
        {
            Assert.Throws<ArgumentNullException>(() => new NameFormatterConfiguration().UpdateFormatter(null));
        }

        [Test]
        public void It_should_update_formatter()
        {
            var nameFormatter = Mock.Of<INameFormatter>();
            var config = new NameFormatterConfiguration().UpdateFormatter(nameFormatter);
            Assert.That(config.GetFormatter(), Is.SameAs(nameFormatter));
        }

        [Test]
        public void Configuration_should_be_sealable()
        {
            var lighbddConfig = new LightBddConfiguration();
            var cfg = lighbddConfig.Get<NameFormatterConfiguration>();
            lighbddConfig.Seal();

            Assert.Throws<InvalidOperationException>(() => cfg.UpdateFormatter(Mock.Of<INameFormatter>()));
        }
    }
}
