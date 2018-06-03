using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Formatting.Values;
using LightBDD.Framework.Parameters;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class ToVerifiable_tests
    {
        [Test]
        public void It_should_convert_to_verifiable()
        {
            var expectation = Expect.To.Not.Empty().And(x => x.All(item => item.Not.Null()));
            Assert.AreEqual(ParameterVerificationStatus.Failure, expectation.ToVerifiable().SetActual(Enumerable.Empty<string>()).Status);
            Assert.AreEqual(ParameterVerificationStatus.Failure, expectation.ToVerifiable<string[]>().SetActual(new string[0]).Status);
            Assert.AreEqual(ParameterVerificationStatus.Failure, expectation.ToVerifiable<string[]>().SetActual(new[] { null, "abc" }).Status);
            Assert.AreEqual(ParameterVerificationStatus.Success, expectation.ToVerifiable<string[]>().SetActual(new[] { "123", "abc" }).Status);
        }
    }
}