namespace LightBDD.Runner.Implementation;

internal static class ReflectionExtensions
{
    public static T[]? ToNullIfEmpty<T>(this T[]? collection) => collection?.Length == 0 ? null : collection;
}