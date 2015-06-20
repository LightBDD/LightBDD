using System;
using LightBDD.Formatting.Parameters;

namespace LightBDD.Example.AcceptanceTests.XUnit.Features
{
    public partial class Invoice_feature : FeatureFixture
    {
        private void Product_is_available_in_products_storage(string product)
        {
        }

        private void Customer_buys_product(string product)
        {
        }

        private void Invoice_is_sent_to_customer()
        {
            throw new IgnoreException("Not implemented yet");
        }

        private void Invoice_contains_product_with_price_of_AMOUNT(string product, [Format("{0:£0.00#}")]decimal amount)
        {
        }
    }
}