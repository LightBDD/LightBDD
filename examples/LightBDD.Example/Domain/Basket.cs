using System.Collections.Generic;

namespace LightBDD.Example.Domain
{
    public class Basket
    {
        private readonly List<string> _products = new List<string>();

        public IEnumerable<string> Products => _products;

        public void Add(string product)
        {
            _products.Add(product);
        }
    }
}