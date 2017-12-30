using System;
using System.Threading.Tasks;
using LightBDD.Core.Formatting.ExceptionFormatting;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Formatting.ExceptionFormatting
{
    [TestFixture]
    public class DefaultExceptionFormatter_tests
    {
        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public void WithStackTraceLinesLimit_should_throw_on_wrong_argument(int limit)
        {
            var ex = Assert.Throws<ArgumentException>(() => new DefaultExceptionFormatter().WithStackTraceLinesLimit(limit));
            Assert.That(ex.Message, Does.StartWith("StackTrace line limit cannot be less than 1"));
        }

        [Test]
        public async Task Format_should_format_exception_with_4_lines_of_stack_trace_by_default()
        {
            var exception = await MakeSampleException();

            var expectedExceptionDetails = @"^System.Exception : ThrowSampleExceptionAsync
	---> System.InvalidOperationException : ThrowInnerExceptionAsync
		---> System.AggregateException : One or more errors occurred.*
			---> System.NotImplementedException : Not implemented yet
			---> System.Exception : other
at LightBDD.Core.UnitTests.Formatting.ExceptionFormatting.DefaultExceptionFormatter_tests.<ThrowSampleExceptionAsync>[^\n]*
--- End of stack trace from previous location where exception was thrown ---
at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw\(\)
at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification\(Task task\)[^\n]*$";

            var formattedDetails = new DefaultExceptionFormatter().Format(exception);
            Assert.That(formattedDetails.Replace("\r", ""), Does.Match(expectedExceptionDetails.Replace("\r", "")));
        }

        [Test]
        public async Task Format_should_format_exception_with_customized_number_of_stack_trace_lines()
        {
            var exception = await MakeSampleException();

            var expectedExceptionDetails = @"^System.Exception : ThrowSampleExceptionAsync
	---> System.InvalidOperationException : ThrowInnerExceptionAsync
		---> System.AggregateException : One or more errors occurred\..*
			---> System.NotImplementedException : Not implemented yet
			---> System.Exception : other
at LightBDD.Core.UnitTests.Formatting.ExceptionFormatting.DefaultExceptionFormatter_tests.<ThrowSampleExceptionAsync>[^\n]*
--- End of stack trace from previous location where exception was thrown ---$";

            var formattedDetails = new DefaultExceptionFormatter().WithStackTraceLinesLimit(2).Format(exception);
            Assert.That(formattedDetails.Replace("\r", ""), Does.Match(expectedExceptionDetails.Replace("\r", "")));
        }

        [Test]
        public async Task Format_should_format_exception_with_filtered_lines()
        {
            var exception = await MakeSampleException();

            var expectedExceptionDetails = @"^System.Exception : ThrowSampleExceptionAsync
	---> System.InvalidOperationException : ThrowInnerExceptionAsync
		---> System.AggregateException : One or more errors occurred\..*
			---> System.NotImplementedException : Not implemented yet
			---> System.Exception : other
at LightBDD.Core.UnitTests.Formatting.ExceptionFormatting.DefaultExceptionFormatter_tests.<ThrowSampleExceptionAsync>[^\n]*
--- End of stack trace from previous location where exception was thrown ---
at LightBDD.Core.UnitTests.Formatting.ExceptionFormatting.DefaultExceptionFormatter_tests.<HandleInnerException>[^\n]*$";

            var formattedDetails = new DefaultExceptionFormatter()
                .WithMembersExcludedFromStackTrace("System.Runtime.*")
                .Format(exception);
            Assert.That(formattedDetails.Replace("\r", ""), Does.Match(expectedExceptionDetails.Replace("\r", "")));
        }

        private async Task<Exception> MakeSampleException()
        {
            return await HandleInnerException();
        }

        private async Task<Exception> HandleInnerException()
        {
            Exception exception = null;
            try
            {
                await ThrowSampleExceptionAsync();
            }
            catch (Exception e)
            {
                exception = e;
            }

            return exception;
        }

        private async Task ThrowSampleExceptionAsync()
        {
            try
            {
                await ThrowInnerExceptionAsync();
            }
            catch (Exception e) { throw new Exception(nameof(ThrowSampleExceptionAsync), e); }
        }

        private async Task ThrowInnerExceptionAsync()
        {
            try
            {
                await ThrowAggregateException();
            }
            catch (Exception e) { throw new InvalidOperationException(nameof(ThrowInnerExceptionAsync), e); }
        }

        private async Task ThrowAggregateException()
        {
            throw new AggregateException(new NotImplementedException("Not implemented yet"), new Exception("other"));
        }
    }
}
