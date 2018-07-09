using System.Collections.Generic;
using System.Linq;

namespace Example.Domain.Domain
{
    public class ContactBook
    {
        private readonly IDictionary<string, Contact> _contacts = new Dictionary<string, Contact>();

        public void AddContact(string name, string phone, string email)
        {
            _contacts.Add(email, new Contact(name, phone, email));
        }

        public void Remove(string email)
        {
            _contacts.Remove(email);
        }

        public IEnumerable<Contact> Contacts => _contacts.Values;

        public IEnumerable<Contact> SearchByPhoneStartingWith(string number)
        {
            return _contacts.Values.Where(x => x.PhoneNumber.StartsWith(number));
        }

        public IEnumerable<Contact> GetNameSortedContacts()
        {
            return Contacts.OrderBy(x => x.Name);
        }
    }
}
