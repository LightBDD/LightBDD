namespace LightBDD.Core.Helpers
{
    internal class Arrays<T>
    {
        private static readonly T[] EmptyArray = new T[0];

        public static T[] Empty()
        {
            return EmptyArray;
        }
    }
}