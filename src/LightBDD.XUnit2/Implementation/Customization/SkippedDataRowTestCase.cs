using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LightBDD.XUnit2.Implementation.Customization
{
    //The SkipReason looks strange but it is a workaround for https://github.com/xunit/xunit/issues/1782
    internal class SkippedDataRowTestCase : XunitTestCase
    {
        [Obsolete("serialization only")]
        public SkippedDataRowTestCase()
        {
        }

        public SkippedDataRowTestCase(IMessageSink diagnosticMessageSink,
            TestMethodDisplay defaultMethodDisplay,
            TestMethodDisplayOptions defaultMethodDisplayOptions,
            ITestMethod testMethod,
            string skipReason,
            object[] testMethodArguments = null)
            : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod, testMethodArguments)
        {
            SkipReason = skipReason;
        }

        public override Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
        {
            return new ScenarioTestCaseRunner(this, DisplayName, SkipReason, constructorArguments, TestMethodArguments, messageBus, aggregator, cancellationTokenSource).RunAsync();
        }

        public override void Deserialize(IXunitSerializationInfo data)
        {
            base.Deserialize(data);
            SkipReason = data.GetValue<string>("SkipReason");
        }

        /// <inheritdoc />
        protected override string GetSkipReason(IAttributeInfo factAttribute)
        {
            return SkipReason;
        }

        /// <inheritdoc />
        public override void Serialize(IXunitSerializationInfo data)
        {
            base.Serialize(data);
            data.AddValue("SkipReason", (object)SkipReason, (Type)null);
        }
    }
}