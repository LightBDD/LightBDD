using System;

namespace Example.Domain.Domain
{
    public class Calculator
    {
        /// <summary>
        /// This is an example of flawed logic that would cause some of the tests to fail.
        /// </summary>
        public int Add(int x, int y)
        {
            return Math.Abs(x + y);
        }

        public int Multiply(int x, int y)
        {
            return x * y;
        }

        public int Divide(int x, int y)
        {
            return x / y;
        }
    }
}
