using Xunit.Abstractions;

namespace LightBDD.Example.AcceptanceTests.XUnit.Features
{
    public partial class Payment_feature : FeatureFixture
    {
        public Payment_feature(ITestOutputHelper output)
            : base(output)
        {
        }
    }
}