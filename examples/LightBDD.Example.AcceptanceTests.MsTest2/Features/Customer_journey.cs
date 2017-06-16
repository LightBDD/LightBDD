using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios.Basic;
using LightBDD.MsTest2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.Example.AcceptanceTests.MsTest2.Features
{
    [TestClass]
    [FeatureDescription(
@"In order to receive a product
As an application user
I want to go through entire customer journey")]
    [Label("Story-6")]
    public partial class Customer_journey : FeatureFixture
    {
        [Scenario]
        [Label("Ticket-12")]
        public async Task Ordering_products()
        {
            await Runner.RunScenarioAsync(
                Given_customer_is_logged_in,
                When_customer_adds_products_to_basket,
                When_customer_pays_for_products_in_basket,
                Then_customer_should_receive_order_email);
        }
    }
}
