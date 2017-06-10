namespace LightBDD.Framework.Extensibility
{
    public static class StepGroupBuilderExtensions
    {
        public static IIntegrableStepGroupBuilder Integrate<TContext>(this IStepGroupBuilder<TContext> builder)
        {
            return (IIntegrableStepGroupBuilder)builder;
        }
    }
}