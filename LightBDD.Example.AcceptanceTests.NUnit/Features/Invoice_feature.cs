using NUnit.Framework;

namespace LightBDD.Example.AcceptanceTests.NUnit.Features
{
    [FeatureDescription(
@"In order to pay for products
As a customer
I want to receive invoice for bought items")]
    [TestFixture]
    [Label("Story-2")]
    public partial class Invoice_feature
    {
        [Test]
        [Label("Ticket-4")]
        [ScenarioCategory(Categories.Sales)]
        public void Receiving_invoice_for_products()
        {
            Runner.RunScenario(
                given => Product_is_available_in_products_storage("wooden desk"),
                and => Product_is_available_in_products_storage("wooden shelf"),
                when => Customer_buys_product("wooden desk"),
                and => Customer_buys_product("wooden shelf"),
                then => Invoice_is_sent_to_customer(),
                and => Invoice_contains_product_with_price_of_AMOUNT("wooden desk", 62),
                and => Invoice_contains_product_with_price_of_AMOUNT("wooden shelf", 37));
        }
    }
}