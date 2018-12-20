using System;

namespace LightBDD.Framework.Fluent
{
	public interface IGiven<T>
	{
		IGiven<T> And(T given);

		IWhen<T> When(T when);
	}
}
