using System;

namespace LightBDD.Framework.Scenarios.Fluent
{
	public interface IThen<T>
	{
		IThen<T> And(T when);

		T[] End();		
	}
}
