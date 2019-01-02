namespace LightBDD.Framework.Scenarios.Fluent
{
	public class GivenResult<TGiven,TWhen>
	{
		public GivenResult(TGiven given, TWhen when)
		{
			this.When = when;
			this.And = given;
		}

		public TGiven And
		{
			get;
		}
		public TWhen When
		{
			get;
		}
	}
}