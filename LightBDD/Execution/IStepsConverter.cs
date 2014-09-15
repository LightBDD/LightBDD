using System;
using System.Collections.Generic;

namespace LightBDD.Execution
{
    internal interface IStepsConverter
    {
        IEnumerable<Step> Convert(IEnumerable<Action> steps);
        IEnumerable<Step> Convert<TContext>(TContext context, IEnumerable<Action<TContext>> steps);
    }
}