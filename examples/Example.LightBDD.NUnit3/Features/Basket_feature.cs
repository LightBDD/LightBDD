using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios.Basic;
using LightBDD.NUnit3;

namespace Example.LightBDD.NUnit3.Features
{
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
        public async Task No_product_in_stock()
        {
            await Runner.RunScenarioActionsAsync(
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
        public async Task Successful_addition()
        {
            await Runner.RunScenarioActionsAsync(
                Given_product_is_in_stock,
                When_customer_adds_it_to_the_basket,
                Then_the_product_addition_should_be_successful,
                Then_the_basket_should_contain_the_product,
                Then_the_product_should_be_removed_from_stock);
        }
    }
}