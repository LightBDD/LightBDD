using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LightBDD.Execution
{
    internal interface IStepsConverter
    {
        IEnumerable<Step> Convert(IEnumerable<Action> steps);
        IEnumerable<Step> Convert<TContext>(TContext context, IEnumerable<Action<TContext>> steps);
        IEnumerable<Step> Convert<TContext>(TContext context, IEnumerable<Expression<Action<TContext>>> steps);
    }
}