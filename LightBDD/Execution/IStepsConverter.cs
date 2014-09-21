using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LightBDD.Execution
{
    internal interface IStepsConverter
    {
        IEnumerable<IStep> Convert(IEnumerable<Action> steps);
        IEnumerable<IStep> Convert<TContext>(TContext context, IEnumerable<Action<TContext>> steps);
        IEnumerable<IStep> Convert<TContext>(TContext context, IEnumerable<Expression<Action<StepType,TContext>>> steps);
        IEnumerable<IStep> Convert(IEnumerable<Expression<Action<StepType>>> steps);
    }
}