using LightBDD.Example.AcceptanceTests.XUnit2.Features.Contexts;

namespace LightBDD.Example.AcceptanceTests.XUnit2.Features
{
    [FeatureDescription(
@"In order to deliver products to customer effectively
As a spedition manager
I want to dispatch products to customer as soon as the payment is finalized")]
    [Label("Story-3")]
    public partial class Product_spedition_feature
    {
        [Scenario]
        [Label("Ticket-5")]
        [ScenarioCategory(Categories.Sales)]
        [ScenarioCategory(Categories.Delivery)]
        public void Should_dispatch_product_after_payment_is_finalized()
        {
            Runner.WithContext<SpeditionContext>().Parameterized().RunScenario(
                given => given.There_is_an_active_customer_with_id("ABC-123"),
                and => and.The_customer_has_product_in_basket("wooden shelf"),
                and => and.The_customer_has_product_in_basket("wooden desk"),
                when => when.The_customer_payment_finalizes(),
                then => then.Product_should_be_dispatched_to_the_customer("wooden shelf"),
                and => and.Product_should_be_dispatched_to_the_customer("wooden desk"));
        }
    }
}