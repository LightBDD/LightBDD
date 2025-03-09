using System.Threading.Tasks;
using LightBDD.Framework;

namespace LightBDD.TUnit.UnitTests;

public class FeatureFixtureTests
{
    private class TestableFeatureFixture : FeatureFixture
    {
        public IBddRunner GetRunner()
        {
            return Runner;
        }
    }

    [Test]
    public async Task Runner_should_be_initialized()
    {
        await Assert.That(new TestableFeatureFixture().GetRunner()).IsNotNull();
    }
}