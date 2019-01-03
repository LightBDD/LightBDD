namespace LightBDD.Framework.Scenarios.Fluent
{
	/// <summary>
	/// If you use context related FluentAPI
	/// <see cref="SceneContext{TGiven, TWhen, TThen, TFunc_Given}"/>
	/// a GIVEN-method has to return an object of this class
	/// </summary>
	/// <typeparam name="TGiven">Type of object that is used for GIVEN-methods</typeparam>
	/// <typeparam name="TWhen">Type of object that is used for WHEN-methods</typeparam>
	public class GivenResult<TGiven,TWhen>
	{
		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="given">object that has the given methods</param>
		/// <param name="when">object that has the when methods</param>
		public GivenResult(TGiven given, TWhen when)
		{
			this.When = when;
			this.And = given;
		}

		/// <summary>
		/// And-link
		/// </summary>
		public TGiven And
		{
			get;
		}
		/// <summary>
		/// When-link
		/// </summary>
		public TWhen When
		{
			get;
		}
	}
}