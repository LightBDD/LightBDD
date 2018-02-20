namespace Example.Domain.Domain
{
	public class Contact
	{
		public string Name { get; }
		public string PhoneNumber { get; }

		public Contact(string name, string phoneNumber)
		{
			PhoneNumber = phoneNumber;
			Name = name;
		}

		#region Equality members
		protected bool Equals(Contact other)
		{
			return string.Equals(Name, other.Name) && string.Equals(PhoneNumber, other.PhoneNumber);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((Contact)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Name?.GetHashCode() ?? 0) * 397) ^ (PhoneNumber?.GetHashCode() ?? 0);
			}
		}

		public override string ToString()
		{
			return $"Name: {Name}, PhoneNumber: {PhoneNumber}";
		}

		#endregion
	}
}