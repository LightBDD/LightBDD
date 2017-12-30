using System;
using LightBDD.Core.Configuration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Configuration
{
    [TestFixture]
    public class ExceptionHandlingConfiguration_tests
    {
        [Test]
        public void It_should_return_default_value()
        {
            Assert.That(new ExceptionHandlingConfiguration().ExceptionDetailsFormatter, Is.Not.Null);
        }

        [Test]
        public void It_should_not_allow_null_value()
        {
            Assert.Throws<ArgumentNullException>(() => new ExceptionHandlingConfiguration().UpdateExceptionDetailsFormatter(null));
        }

        [Test]
        public void It_should_update_value()
        {
            var formattedText = "something";
            var config = new ExceptionHandlingConfiguration().UpdateExceptionDetailsFormatter(ex => formattedText);
            Assert.That(config.ExceptionDetailsFormatter(new Exception()), Is.EqualTo(formattedText));
        }

        [Test]
        public void Configuration_should_be_sealable()
        {
            var config = new LightBddConfiguration();
            var cfg = config.Get<ExceptionHandlingConfiguration>();
            config.Seal();

            Assert.Throws<InvalidOperationException>(() => cfg.UpdateExceptionDetailsFormatter(ex => "abc"));
        }
    }
}