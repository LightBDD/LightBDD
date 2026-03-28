using System;
using System.Collections.Generic;
using Xunit.Sdk;
using Xunit.v3;

namespace LightBDD.XUnit3.Implementation.Customization
{
    /// <summary>
    /// Custom delay-enumerated theory test case that intercepts xUnit's skip mechanism.
    /// Used as fallback when data cannot be pre-enumerated at discovery time.
    /// When a [Scenario(Skip = "...")] is used on a parameterized scenario,
    /// this test case stores the skip reason but does NOT pass it to xUnit's skip infrastructure.
    /// </summary>
    internal class ScenarioTheoryTestCase : XunitDelayEnumeratedTheoryTestCase
    {
        internal string OriginalSkipReason { get; private set; }

        [Obsolete("For deserialization only")]
        public ScenarioTheoryTestCase() { }

        public ScenarioTheoryTestCase(
            IXunitTestMethod testMethod,
            string testCaseDisplayName,
            string uniqueID,
            bool @explicit,
            bool skipTestWithoutData,
            string originalSkipReason,
            Dictionary<string, HashSet<string>> traits = null,
            string sourceFilePath = null,
            int? sourceLineNumber = null,
            int? timeout = null)
            : base(
                testMethod,
                testCaseDisplayName,
                uniqueID,
                @explicit,
                skipTestWithoutData,
                skipExceptions: null,
                skipReason: null,
                skipType: null,
                skipUnless: null,
                skipWhen: null,
                traits: traits,
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
