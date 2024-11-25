using ContactManager.AppLogic.Contracts;
using ContactManager.Domain;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Infrastructure
{
    internal class ContactDbRepository:IContactRepository
    {
        public IList<Contact> GetAllContacts()
        {
            throw new NotImplementedException();

        }
        public void AddContact(Contact contact)
        {
            throw new NotImplementedException();
        }
    }
}
