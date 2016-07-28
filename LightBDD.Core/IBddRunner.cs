namespace LightBDD
{
    public interface IBddRunner<TContext> { }

    public sealed class NoContext { }

    public interface IBddRunner : IBddRunner<NoContext> { }
}
