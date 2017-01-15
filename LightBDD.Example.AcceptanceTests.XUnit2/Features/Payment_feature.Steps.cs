using Xunit.Abstractions;

namespace LightBDD.Example.AcceptanceTests.XUnit2.Features
{
    public partial class Payment_feature : FeatureFixture
    {
        #region Setup/Teardown
        public Payment_feature(ITestOutputHelper output)
            : base(output)
        {
        }
        #endregion
    }
}