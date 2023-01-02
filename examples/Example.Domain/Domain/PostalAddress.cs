namespace Example.Domain.Domain
{
    public class PostalAddress
    {
        public string Address { get; }
        public string City { get; }
        public string PostCode { get; }
        public string Country { get; }

        public PostalAddress(string country, string city, string postCode, string address)
        {
            Address = address;
            City = city;
            PostCode = postCode;
            Country = country;
        }
    }
}