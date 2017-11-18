using System.Linq;
using Example.Domain.Domain;
using LightBDD.Framework;
using LightBDD.Framework.Commenting;
using LightBDD.MsTest2;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Example.LightBDD.MsTest2.Features
{
    public partial class Basket_feature : FeatureFixture
    {
        private Stock _stock;
        private Basket _basket;
        private bool _transferResult;

        private void Given_product_is_in_stock()
        {
            var product = "product";
            _stock.Add(product);
            StepExecution.Current.Comment(string.Format("Added '{0}' to the stock", product));
        }

        private void Given_product_is_out_of_stock()
        {
            //It is not added, so it is out of stock
        }

        private async void When_customer_adds_it_to_the_basket()
        {
            var product = "product";
            StepExecution.Current.Comment(string.Format("Transferring '{0}' to the basket", product));
            _transferResult = await _stock.TransferToBasketAsync(_basket, product);
        }

        private void Then_the_product_addition_should_be_successful()
        {
            Assert.IsTrue(_transferResult);
        }

        private void Then_the_basket_should_contain_the_product()
        {
            Assert.IsTrue(Enumerable.Contains(_basket.Products, "product"));
        }

        private void Then_the_product_addition_should_be_unsuccessful()
        {
            Assert.IsFalse(_transferResult);
        }

        private void Then_the_basket_should_not_contain_the_product()
        {
            Assert.IsFalse(Enumerable.Contains(_basket.Products, "product"));
        }

        private void Then_the_product_should_be_removed_from_stock()
        {
            Assert.Inconclusive("Product removal from stock is not implemented yet");
            Assert.IsFalse(Enumerable.Contains(_stock.Products, "product"));
        }

        #region Setup/Teardown

        [TestInitialize]
        public void SetUp()
        {
            _stock = new Stock();
            _basket = new Basket();
        }

        #endregion
    }
}