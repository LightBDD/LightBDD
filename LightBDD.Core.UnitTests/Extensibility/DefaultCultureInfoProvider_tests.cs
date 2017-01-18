using System.Globalization;
using LightBDD.Core.Extensibility;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Extensibility
{
    [TestFixture]
    public class DefaultCultureInfoProvider_tests
    {
        [Test]
        public void It_should_return_invariant_culture()
        {
            Assert.That(new DefaultCultureInfoProvider().GetCultureInfo(), Is.SameAs(CultureInfo.InvariantCulture));
        }
    }
}
