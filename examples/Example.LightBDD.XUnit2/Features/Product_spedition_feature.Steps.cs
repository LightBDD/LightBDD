using LightBDD.XUnit2;
using Xunit.Abstractions;

namespace Example.LightBDD.XUnit2.Features
{
    public partial class Product_spedition_feature : FeatureFixture
    {
        #region Setup/Teardown
        public Product_spedition_feature(ITestOutputHelper output)
            : base(output)
        {
        }
        #endregion
    }
}