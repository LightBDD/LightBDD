namespace Example.Domain.Domain
{
    public class Contact
    {
        public string Name { get; }
        public string PhoneNumber { get; }
        public string Email { get; }

        public Contact(string name, string phoneNumber, string email)
        {
            PhoneNumber = phoneNumber;
            Email = email;
            Name = name;
        }

        #region Equality members

        protected bool Equals(Contact other)
        {
            return string.Equals(Name, other.Name) && string.Equals(PhoneNumber, other.PhoneNumber) && string.Equals(Email, other.Email);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Contact)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PhoneNumber != null ? PhoneNumber.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Email != null ? Email.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"Name: {Name}, PhoneNumber: {PhoneNumber}, Email: {Email}";
        }

        #endregion
    }
}