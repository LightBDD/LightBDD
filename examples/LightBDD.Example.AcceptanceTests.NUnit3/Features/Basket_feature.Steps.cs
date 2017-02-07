using System.Linq;
using LightBDD.Example.Domain;
using LightBDD.Framework;
using LightBDD.Framework.Commenting;
using LightBDD.NUnit3;
using NUnit.Framework;

namespace LightBDD.Example.AcceptanceTests.NUnit3.Features
{
    public partial class Basket_feature : FeatureFixture
    {
        private Stock _stock;
        private Basket _basket;
        private bool _transferResult;
        private static readonly string Product = "product";

        private void Given_product_is_in_stock()
        {
            _stock.Add(Product);
            StepExecution.Current.Comment(string.Format("Added '{0}' to the stock", Product));
        }

        private void Given_product_is_out_of_stock()
        {
            //It is not added, so it is out of stock
        }

        private async void When_customer_adds_it_to_the_basket()
        {
            StepExecution.Current.Comment(string.Format("Transferring '{0}' to the basket", Product));
            _transferResult = await _stock.TransferToBasketAsync(_basket, Product);
        }

        private void Then_the_product_addition_should_be_successful()
        {
            Assert.That(_transferResult, Is.True);
        }

        private void Then_the_basket_should_contain_the_product()
        {
            Assert.That(_basket.Products.Contains(Product), Is.True);
        }

        private void Then_the_product_addition_should_be_unsuccessful()
        {
            Assert.That(_transferResult, Is.False);
        }

        private void Then_the_basket_should_not_contain_the_product()
        {
            Assert.That(_basket.Products.Contains(Product), Is.False);
        }

        private void Then_the_product_should_be_removed_from_stock()
        {
            Assert.Ignore("Product removal from stock is not implemented yet");
            Assert.That(_stock.Products.Contains(Product), Is.False);
        }

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _stock = new Stock();
            _basket = new Basket();
        }

        #endregion
    }
}