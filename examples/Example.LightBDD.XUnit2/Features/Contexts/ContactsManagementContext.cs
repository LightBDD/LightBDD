using System.Globalization;
using System.Linq;
using Example.Domain.Domain;
using LightBDD.Framework;
using Xunit;

namespace Example.LightBDD.XUnit2.Features.Contexts
{
    public class ContactsManagementContext
    {
        public ContactBook ContactBook { get; set; }
        public Contact[] AddedContacts { get; set; }
        public Contact[] RemovedContacts { get; set; }

        public ContactsManagementContext()
        {
            ContactBook = new ContactBook();
            AddedContacts = new Contact[0];
            RemovedContacts = new Contact[0];
        }

        public void Given_my_contact_book_is_empty()
        {
            ContactBook = new ContactBook();
        }

        public void When_I_add_new_contacts()
        {
            AddSomeContacts();
        }

        public void Then_all_contacts_should_be_available_in_the_contact_book()
        {
            Assert.Equal(
                   AddedContacts,
                   ContactBook.Contacts.ToArray());
        }

        public void Given_my_contact_book_is_filled_with_contacts()
        {
            ContactBook = new ContactBook();
            AddSomeContacts();
        }

        public void When_I_remove_one_contact()
        {
            RemovedContacts = ContactBook.Contacts.Take(1).ToArray();
            foreach (var contact in RemovedContacts)
                ContactBook.Remove(contact.Name);
        }

        public void Then_the_contact_book_should_not_contain_removed_contact_any_more()
        {
            Assert.Equal(
                 Enumerable.Empty<Contact>(),
                 ContactBook.Contacts.Where(c => RemovedContacts.Contains(c)).ToArray());
        }

        public void Then_the_contact_book_should_contains_all_other_contacts()
        {
            Assert.Equal(
                   ContactBook.Contacts.ToArray(),
                   AddedContacts.Except(RemovedContacts).ToArray());
        }

        public void Given_my_contact_book_is_filled_with_many_contacts()
        {
            for (var i = 0; i < 10000; ++i)
                ContactBook.AddContact(i.ToString(CultureInfo.InvariantCulture), i.ToString(CultureInfo.InvariantCulture));
        }

        public void When_I_clear_it()
        {
            foreach (var contact in ContactBook.Contacts.ToArray())
                ContactBook.Remove(contact.Name);
            StepExecution.Current.Bypass("Contact book clearing is not implemented yet. Contacts are removed one by one.");
        }

        public void Then_the_contact_book_should_be_empty()
        {
            Assert.Empty(ContactBook.Contacts);
        }

        private void AddSomeContacts()
        {
            AddedContacts = new[]
            {
                new Contact("Jack", "123-456-789"),
                new Contact("Samantha", "321-654-987"),
                new Contact("Josh", "132-465-798")
            };

            foreach (var contact in AddedContacts)
                ContactBook.AddContact(contact.Name, contact.PhoneNumber);
        }
    }
}