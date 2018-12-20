using System;

namespace LightBDD.Framework.Fluent
{
	public interface IWhen<T>
	{
		IWhen<T> And(T when);

		IThen<T> Then(T then);
	}
}
