namespace LightBDD.Example.AcceptanceTests.XUnit.Features
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
        public void No_product_in_stock()
        {
            Runner.RunScenario(
                Given_product_is_out_of_stock,
                When_customer_adds_it_to_basket,
                Then_product_addition_is_unsuccessful,
                Then_basket_does_not_contain_product);
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
                When_customer_adds_it_to_basket,
                Then_product_addition_is_successful,
                Then_basket_contains_product,
                Then_product_is_removed_from_stock);
        }
    }
}