
using Xunit.Abstractions;

namespace LightBDD.Example.AcceptanceTests.XUnit2.Features
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