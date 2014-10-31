namespace LightBDD.Execution.Parameters
{
    internal interface IStepParameter<TContext>
    {
        object Evaluate(TContext context);
        object GetNotEvaluatedValue();
    }
}