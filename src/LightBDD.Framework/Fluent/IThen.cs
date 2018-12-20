using System;

namespace LightBDD.Framework.Fluent
{
	public interface IThen<T>
	{
		IThen<T> And(T when);

		T[] End();
	}
}
