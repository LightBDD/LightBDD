using System.Collections.Generic;

namespace Example.Domain.Domain
{
    public class ContactBook
    {
        private readonly IDictionary<string, Contact> _contacts = new Dictionary<string, Contact>();

        public void AddContact(string name, string phone)
        {
            _contacts.Add(name, new Contact(name, phone));
        }

        public void Remove(string name)
        {
            _contacts.Remove(name);
        }

        public IEnumerable<Contact> Contacts => _contacts.Values;
    }
}
