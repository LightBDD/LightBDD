namespace LightBDD.Framework
{
    /// <summary>
    /// The base runner interface describing runner that can execute scenarios within specified context.
    /// See also: <seealso cref="IBddRunner"/>.
    /// </summary>
    /// <typeparam name="TContext">The context type that would be used in scenario execution.</typeparam>
    public interface IBddRunner<TContext> { }

    /// <summary>
    /// Main LightBDD runner interface that should be used in all LightBDD tests.
    /// The interface describes the runner with no context - please browse "LightBDD.Framework.Scenarios.Contextual" namespace for contextual runners extension methods.
    /// <param>The runner instance can be obtained by installing package from "LightBDD.Integration.*" group and deriving test class from <c>FeatureFixture</c> class offered by integration package.</param>
    /// <param>The "LightBDD.Framework.Scenarios.*" namespaces offers a set of extension methods to execute tests with this runner.</param>
    /// </summary>
    public interface IBddRunner : IBddRunner<NoContext> { }
}
