using System.Collections.Generic;

namespace Example.Domain.Domain
{
    public class AddressBook
    {
        private readonly Dictionary<string, ContactAddress> _contactsByEmail = new Dictionary<string, ContactAddress>();
        public IReadOnlyDictionary<string, ContactAddress> ContactsByEmail => _contactsByEmail;

        public void AddContactAddress(ContactAddress contact) { _contactsByEmail[contact.Contact.Email] = contact; }
    }
}