namespace Example.Domain.Domain
{
    public class ContactAddress
    {
        public string Alias { get; set; }
        public Contact Contact { get; }
        public PostalAddress Address { get; set; }

        public ContactAddress(string alias,Contact contact, PostalAddress address)
        {
            Alias = alias;
            Contact = contact;
            Address = address;
        }
    }
}