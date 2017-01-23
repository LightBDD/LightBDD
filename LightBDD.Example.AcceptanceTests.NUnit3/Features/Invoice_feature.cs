using LightBDD.Scenarios.Extended;

namespace LightBDD.Example.AcceptanceTests.NUnit3.Features
{
    [FeatureDescription(
@"In order to pay for products
As a customer
I want to receive invoice for bought items")]
    [Label("Story-2")]
    public partial class Invoice_feature
    {
        [Scenario]
        [Label("Ticket-4")]
        [ScenarioCategory(Categories.Sales)]
        public void Receiving_invoice_for_products()
        {
            Runner.Parameterized().RunScenario(
                given => Product_is_available_in_product_storage("wooden desk"),
                and => Product_is_available_in_product_storage("wooden shelf"),
                when => Customer_buys_product("wooden desk"),
                and => Customer_buys_product("wooden shelf"),
                then => An_invoice_should_be_sent_to_the_customer(),
                and => The_invoice_should_contain_product_with_price_of_AMOUNT("wooden desk", 62),
                and => The_invoice_should_contain_product_with_price_of_AMOUNT("wooden shelf", 37));
        }
    }
}