﻿using LightBDD.Framework.Formatting;
using LightBDD.NUnit2;
using NUnit.Framework;

namespace Example.LightBDD.NUnit2.Features
{
    public partial class Invoice_feature : FeatureFixture
    {
        private void Given_product_is_available_in_product_storage(string product)
        {
        }

        private void When_customer_buys_product(string product)
        {
        }

        private void Then_an_invoice_should_be_sent_to_the_customer()
        {
            Assert.Inconclusive("Not implemented yet");
        }

        private void Then_the_invoice_should_contain_product_with_price_of_AMOUNT(string product, [Format("{0:£0.00#}")]decimal amount)
        {
        }
    }
}