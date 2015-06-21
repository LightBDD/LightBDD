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
                (and, ctx) => ctx.The_customer_has_product_in_basket("wooden shelf"),
                (and, ctx) => ctx.The_customer_has_product_in_basket("wooden desk"),
                (when, ctx) => ctx.The_customer_payment_finalizes(),
                (then, ctx) => ctx.Product_should_be_dispatched_to_the_customer("wooden shelf"),
                (and, ctx) => ctx.Product_should_be_dispatched_to_the_customer("wooden desk"));
        }
    }

    public class SpeditionContext
    {
        public void The_customer_has_product_in_basket(string product)
        {
        }

        public void The_customer_payment_finalizes()
        {
        }

        public void Product_should_be_dispatched_to_the_customer(string product)
        {
        }

        public void There_is_an_active_customer_with_id(string id)
        {
        }
    }
}