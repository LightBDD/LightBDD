using RandomTestValues;

namespace LightBDD.UnitTests.Helpers
{
    public class Fake
    {
        private static readonly RandomValueSettings Settings = new() { LengthOfCollection = 2 };
        public static T Object<T>() where T : new() => RandomValue.Object<T>(Settings);
        public static string String() => RandomValue.String();
        public static int Int() => RandomValue.Int();
        public static string[] StringArray() => new[] { String(), String() };
    }
}