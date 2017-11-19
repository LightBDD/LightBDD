using LightBDD.XUnit2;
using Xunit.Abstractions;

namespace Example.LightBDD.XUnit2.Features
{
    public partial class Contacts_management : FeatureFixture
    {
        #region Setup/Teardown
        public Contacts_management(ITestOutputHelper output) : base(output)
        {
        }
        #endregion
    }
}