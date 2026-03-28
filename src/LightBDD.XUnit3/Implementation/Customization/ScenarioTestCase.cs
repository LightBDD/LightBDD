using System;
using System.Collections.Generic;
using Xunit.Sdk;
using Xunit.v3;

namespace LightBDD.XUnit3.Implementation.Customization
{
    /// <summary>
    /// Custom test case that intercepts xUnit's skip mechanism.
    /// When a [Scenario(Skip = "...")] or [InlineData(Skip = "...")] is used,
    /// this test case stores the skip reason but does NOT pass it to xUnit's skip infrastructure.
    /// Instead, the test executes normally and LightBDD's TestSkippedDecorator throws IgnoreException at runtime,
    /// so the skip is recorded in LightBDD reports.
    /// </summary>
    internal class ScenarioTestCase : XunitTestCase
    {
        internal string OriginalSkipReason { get; private set; }

        [Obsolete("For deserialization only")]
        public ScenarioTestCase() { }

        public ScenarioTestCase(
            IXunitTestMethod testMethod,
            string testCaseDisplayName,
            string uniqueID,
            bool @explicit,
            string originalSkipReason,
            Dictionary<string, HashSet<string>> traits = null,
            object[] testMethodArguments = null,
            string sourceFilePath = null,
            int? sourceLineNumber = null,
            int? timeout = null)
            : base(
                testMethod,
                testCaseDisplayName,
                uniqueID,
                @explicit,
                skipExceptions: null,
                skipReason: null,
                skipType: null,
                skipUnless: null,
                skipWhen: null,
                traits: traits,
                testMethodArguments: testMethodArguments,
                sourceFilePath: sourceFilePath,
                sourceLineNumber: sourceLineNumber,
                timeout: timeout)
        {
            OriginalSkipReason = originalSkipReason;
        }

        public override void PreInvoke()
        {
            base.PreInvoke();
            if (!string.IsNullOrWhiteSpace(OriginalSkipReason))
                SkipReasonProvider.Current = OriginalSkipReason;
        }

        public override void PostInvoke()
        {
            SkipReasonProvider.Current = null;
            base.PostInvoke();
        }

        protected override void Serialize(IXunitSerializationInfo info)
        {
            base.Serialize(info);
            info.AddValue("OriginalSkipReason", OriginalSkipReason);
        }

        protected override void Deserialize(IXunitSerializationInfo info)
        {
            base.Deserialize(info);
            OriginalSkipReason = info.GetValue<string>("OriginalSkipReason");
        }
    }
}
