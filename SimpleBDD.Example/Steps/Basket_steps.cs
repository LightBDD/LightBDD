// ReSharper disable CheckNamespace

using System.Linq;
using NUnit.Framework;
using SimpleBDD.Example.Domain;

namespace SimpleBDD.Example
{
	public partial class Basket_feature : FeatureTestsBase
	{
		private Stock _stock;
		private Basket _basket;
		private bool _transferResult;

		private void Given_product_is_in_stock()
		{
			_stock.Add("product");
		}

		private void When_customer_adds_it_to_basket()
		{
			_transferResult = _stock.TransferToBasket(_basket, "product");
		}

		private void Then_product_addition_is_successful()
		{
			Assert.That(_transferResult, Is.True);
		}

		private void Then_basket_contains_product()
		{
			Assert.That(_basket.Products.Contains("product"), Is.True);
		}

		private void Given_product_is_out_of_stock()
		{			
			//It is not added, so it is out of stock
		}

		private void Then_product_addition_is_unsuccessful()
		{
			Assert.That(_transferResult, Is.False);
		}

		private void Then_basket_does_not_contain_product()
		{
			Assert.That(_basket.Products.Contains("product"), Is.False);
		}

		private void Then_product_is_removed_from_stock()
		{
			Assert.That(_stock.Products.Contains("product"), Is.False);
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
// ReSharper restore CheckNamespace