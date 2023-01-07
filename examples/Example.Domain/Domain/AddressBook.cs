using System.Collections.Generic;
using System.Linq;

namespace Example.Domain.Domain
{
    public class AddressBook
    {
        private readonly Dictionary<string, ContactAddress> _contactsByEmail = new Dictionary<string, ContactAddress>();
        public IReadOnlyDictionary<string, ContactAddress> ContactsByEmail => _contactsByEmail;

        public void AddContactAddress(ContactAddress contact) { _contactsByEmail[contact.Contact.Email] = contact; }

        public IEnumerable<Contact> GetContacts() => _contactsByEmail.Values.Select(x => x.Contact).OrderBy(x => x.Name);

        public IEnumerable<PostalAddress> GetAddresses() => _contactsByEmail.Values.Select(x => x.Address).OrderBy(x => x.Country).ThenBy(x => x.PostCode);
    }
}