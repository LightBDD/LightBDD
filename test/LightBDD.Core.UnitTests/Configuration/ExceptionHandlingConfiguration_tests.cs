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
        public void It_should_format_exception_with_default_formatter()
        {
            var exception = MakeSampleException();

            var expectedExceptionDetails = @"^System\.Exception : ThrowSampleException
	---> System.InvalidOperationException : ThrowInnerException
		---> System\.AggregateException : One or more errors occurred\..*
			---> System\.NotImplementedException : Not implemented yet
			---> System\.Exception : other
at LightBDD\.Core\.UnitTests\.Configuration\.ExceptionHandlingConfiguration_tests\.ThrowSampleException\(\) in";
            var formattedDetails = new ExceptionHandlingConfiguration().ExceptionDetailsFormatter(exception);
            Assert.That(formattedDetails.Replace("\r", ""), Does.Match(expectedExceptionDetails.Replace("\r", "")));
        }

        private Exception MakeSampleException()
        {
            Exception exception = null;
            try
            {
                ThrowSampleException();
            }
            catch (Exception e)
            {
                exception = e;
            }
            return exception;
        }

        private void ThrowSampleException()
        {
            try
            {
                ThrowInnerException();
            }
            catch (Exception e) { throw new Exception(nameof(ThrowSampleException), e); }
        }

        private void ThrowInnerException()
        {
            try
            {
                ThrowAggregateException();
            }
            catch (Exception e) { throw new InvalidOperationException(nameof(ThrowInnerException), e); }
        }

        private void ThrowAggregateException()
        {
            throw new AggregateException(new NotImplementedException("Not implemented yet"), new Exception("other"));
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