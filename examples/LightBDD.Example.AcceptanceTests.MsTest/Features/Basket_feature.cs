using LightBDD.Framework;
using LightBDD.Framework.Scenarios.Basic;
using LightBDD.MsTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.Example.AcceptanceTests.MsTest.Features
{
    [TestClass]
    [FeatureDescription(
@"In order to buy products
As a customer
I want to add products to basket")]
    [Label("Story-4")]
    public partial class Basket_feature
    {
        [Scenario]
        [Label("Ticket-6")]
        [ScenarioCategory(Categories.Sales)]
        public void No_product_in_stock()
        {
            Runner.RunScenario(
                Given_product_is_out_of_stock,
                When_customer_adds_it_to_the_basket,
                Then_the_product_addition_should_be_unsuccessful,
                Then_the_basket_should_not_contain_the_product);
        }

        /// <summary>
        /// This test presents how LightBDD treats tests with Inconclusive / Ignore asserts
        /// </summary>
        [Scenario]
        [Label("Ticket-7")]
        [ScenarioCategory(Categories.Sales)]
        public void Successful_addition()
        {
            Runner.RunScenario(
                Given_product_is_in_stock,
                When_customer_adds_it_to_the_basket,
                Then_the_product_addition_should_be_successful,
                Then_the_basket_should_contain_the_product,
                Then_the_product_should_be_removed_from_stock);
        }
    }
}