using NUnit.Framework;

namespace LightBDD.Example.AcceptanceTests.NUnit.Features
{
    [FeatureDescription(
@"In order to deliver products to customer effectively
As a spedition manager
I want to dispatch products to customer as soon as the payment is finalized")]
    [Label("Story-3")]
    [TestFixture]
    public class Product_spedition_feature : FeatureFixture
    {
        [Test]
        [Label("Ticket-5")]
        [ScenarioCategory(Categories.Sales)]
        [ScenarioCategory(Categories.Delivery)]
        public void Should_dispatch_product_after_payment_is_finalized()
        {
            Runner.RunScenario<SpeditionContext>(
                (given, ctx) => ctx.There_is_an_active_customer_with_id("ABC-123"),
                (given, ctx) => ctx.Customer_has_product_in_basket("wooden shelf"),
                (given, ctx) => ctx.Customer_has_product_in_basket("wooden desk"),
                (when, ctx) => ctx.Customer_payment_has_been_finalized(),
                (then, ctx) => ctx.Product_has_been_dispatched_to_customer("wooden shelf"),
                (then, ctx) => ctx.Product_has_been_dispatched_to_customer("wooden desk"));
        }
    }

    public class SpeditionContext
    {
        public void Customer_has_product_in_basket(string product)
        {
        }

        public void Customer_payment_has_been_finalized()
        {
        }

        public void Product_has_been_dispatched_to_customer(string product)
        {
        }

        public void There_is_an_active_customer_with_id(string id)
        {
        }
    }
}