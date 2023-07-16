using LightBDD.Core.Configuration;
using LightBDD.Core.Formatting;
using Moq;
using NUnit.Framework;
using System;
using Shouldly;

namespace LightBDD.Framework.UnitTests.Formatting.Configuration
{
    [TestFixture]
    public class NameFormatterConfiguration_tests
    {
        [Test]
        public void It_should_return_dummy_formatter_by_default()
        {
            var formatter = new NameFormatterConfiguration().GetFormatter();
            formatter.FormatName("abc_def ghi").ShouldBe("abc_def ghi");
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
