using System;
using System.Threading.Tasks;

namespace LightBDD.Scenarios.Parameterized.UnitTests.Helpers
{
    public class MyContext
    {
        public void AssertIsSameAs(MyContext other)
        {
            if (this != other)
                throw new ArgumentException("Instances are not the same");
        }

        public async Task AssertIsSameAsAsync(MyContext other)
        {
            await Task.Yield();
            if (this != other)
                throw new ArgumentException("Instances are not the same");
        }
    }
}