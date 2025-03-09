using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;

namespace Example.LightBDD.TUnit.Features
{
    /// <summary>
    /// With TUnit integration it is suggested to use async steps with `async Task` signature to leverage TUnit `Assert.That` framework capabilities.
    /// </summary>
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
            await Runner.AddAsyncSteps(
                    Given_product_is_out_of_stock,
                    When_customer_adds_it_to_the_basket,
                    Then_the_product_addition_should_be_unsuccessful,
                    Then_the_basket_should_not_contain_the_product)
                .RunAsync();
        }

        /// <summary>
        /// This test presents how LightBDD treats tests with Inconclusive / Ignore assertions.
        /// </summary>
        [Scenario]
        [Label("Ticket-7")]
        [ScenarioCategory(Categories.Sales)]
        [ScenarioDescription("This scenario presents how LightBDD reports ignored steps")]
        public async Task Successful_addition()
        {
            await Runner.AddAsyncSteps(
                    Given_product_is_in_stock,
                    When_customer_adds_it_to_the_basket,
                    Then_the_product_addition_should_be_successful,
                    Then_the_basket_should_contain_the_product,
                    Then_the_product_should_be_removed_from_stock)
                .RunAsync();
        }
    }
}