using LightBDD.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.Example.AcceptanceTests.MsTest.Features
{
    [TestClass]
    [FeatureDescription(
@"In order to get desired products
As a customer
I want to pay for products in basket")]
    [Label("Story-5")]
    public partial class Payment_feature
    {
    }
}