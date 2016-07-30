using System;
using LightBDD.Configuration;
using LightBDD.Core.Formatting;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.Core.UnitTests.Configuration
{
    [TestFixture]
    public class NameFormatterConfiguration_tests
    {
        [Test]
        public void It_should_return_default_name_formatter()
        {
            Assert.That(new NameFormatterConfiguration().Formatter, Is.InstanceOf<DefaultNameFormatter>());
        }

        [Test]
        public void It_should_not_allow_null_formatter()
        {
            Assert.Throws<ArgumentNullException>(() => new NameFormatterConfiguration().UpdateFormatter(null));
        }

        [Test]
        public void It_should_update_formatter()
        {
            var nameFormatter = MockRepository.GenerateMock<INameFormatter>();
            var config = new NameFormatterConfiguration().UpdateFormatter(nameFormatter);
            Assert.That(config.Formatter, Is.SameAs(nameFormatter));
        }
    }
}
