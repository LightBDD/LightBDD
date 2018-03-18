using System.Collections.Generic;
using System.Linq;

namespace Example.Domain.Domain
{
    public class ContactBook
    {
        private readonly IDictionary<string, Contact> _contacts = new Dictionary<string, Contact>();

        public void AddContact(string name, string phone, string email)
        {
            _contacts.Add(name, new Contact(name, phone, email));
        }

        public void Remove(string name)
        {
            _contacts.Remove(name);
        }

        public IEnumerable<Contact> Contacts => _contacts.Values;

        public IEnumerable<Contact> SearchByPhoneStartingWith(string number)
        {
            return _contacts.Values.Where(x => x.PhoneNumber.StartsWith(number));
        }
    }
}
