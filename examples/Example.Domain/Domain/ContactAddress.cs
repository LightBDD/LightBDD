namespace Example.Domain.Domain
{
    public class ContactAddress
    {
        public Contact Contact { get; }
        public PostalAddress Address { get; }

        public ContactAddress(Contact contact, PostalAddress address)
        {
            Contact = contact;
            Address = address;
        }
    }
}