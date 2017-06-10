namespace LightBDD.Framework
{
    public interface IStepGroupBuilder<TContext>
    {
        StepGroup Build();
    }
}