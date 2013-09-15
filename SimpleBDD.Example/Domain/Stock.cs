using System.Collections.Generic;

namespace SimpleBDD.Example.Domain
{
	public class Stock
	{
		private readonly List<string> _products = new List<string>();

		public IEnumerable<string> Products
		{
			get { return _products; }
		}

		public void Add(string product)
		{
			_products.Add(product);
		}

		public bool TransferToBasket(Basket basket, string product)
		{
			if (!_products.Contains(product) || !_products.Remove(product)) return false;
			basket.Add(product);
			return true;
		}
	}
}