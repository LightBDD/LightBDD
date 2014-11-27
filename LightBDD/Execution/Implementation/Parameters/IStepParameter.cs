namespace LightBDD.Execution.Implementation.Parameters
{
    internal interface IStepParameter<TContext>
    {
        void Evaluate(TContext context);
        bool IsEvaluated { get; }
        object Value { get; }
        string Format();
    }
}