namespace LightBDD.Framework
{
    public interface IStepGroupBuilder<TContext>
    {
        StepGroup Build();
    }

    public interface IStepGroupBuilder : IStepGroupBuilder<NoContext>
    {

    }
}