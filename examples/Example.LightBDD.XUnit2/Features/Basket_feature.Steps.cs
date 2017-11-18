using System.Linq;
using Example.Domain.Domain;
using LightBDD.Framework;
using LightBDD.Framework.Commenting;
using LightBDD.XUnit2;
using Xunit;
using Xunit.Abstractions;

namespace Example.LightBDD.XUnit2.Features
{
    public partial class Basket_feature : FeatureFixture
    {
        private readonly Stock _stock;
        private readonly Basket _basket;
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
            Assert.True(_transferResult);
        }

        private void Then_the_basket_should_contain_the_product()
        {
            Assert.True(Enumerable.Contains(_basket.Products, "product"));
        }

        private void Then_the_product_addition_should_be_unsuccessful()
        {
            Assert.False(_transferResult);
        }

        private void Then_the_basket_should_not_contain_the_product()
        {
            Assert.False(Enumerable.Contains(_basket.Products, "product"));
        }

        private void Then_the_product_should_be_removed_from_stock()
        {
            StepExecution.Current.IgnoreScenario("Product removal from stock is not implemented yet");
            Assert.False(Enumerable.Contains(_stock.Products, "product"));
        }

        #region Setup/Teardown

        public Basket_feature(ITestOutputHelper output):base(output)
        {
            _stock = new Stock();
            _basket = new Basket();
        }

        #endregion
    }
}