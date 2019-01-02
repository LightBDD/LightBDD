namespace LightBDD.Framework.Scenarios.Fluent
{
	public class GivenResult<G,W>
	{
		public GivenResult(G given, W when)
		{
			this.When = when;
			this.And = given;
		}

		public G And
		{
			get;
		}
		public W When
		{
			get;
		}
	}
}