using System;

namespace LightBDD.Example.Domain
{
    public class Calculator
    {
        public int Add(int x, int y) => Math.Abs(x + y);
        public int Multiply(int x, int y) => x * y;
        public int Divide(int x, int y) => x / y;
    }
}
