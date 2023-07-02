using System;
using System.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Framework.Expectations;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class ToVerifiable_tests
    {
        [Test]
        public void It_should_convert_to_verifiable()
        {
            var expectation = Expect.To.Not.BeEmpty().And(x => x.EveryItem(item => item.Not.BeNull()));
            Assert.AreEqual(ParameterVerificationStatus.Failure, expectation.ToVerifiable().SetActual(Enumerable.Empty<string>()).Status);
            Assert.AreEqual(ParameterVerificationStatus.Failure, expectation.ToVerifiable<string[]>().SetActual(Array.Empty<string>()).Status);
            Assert.AreEqual(ParameterVerificationStatus.Failure, expectation.ToVerifiable<string[]>().SetActual(new[] { null, "abc" }).Status);
            Assert.AreEqual(ParameterVerificationStatus.Success, expectation.ToVerifiable<string[]>().SetActual(new[] { "123", "abc" }).Status);
        }
    }
}