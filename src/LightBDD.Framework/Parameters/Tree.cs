#nullable enable
namespace LightBDD.Framework.Parameters;

public static class Tree
{
    public static InputTree<TData> For<TData>(TData data) => new(data);
}