using ContactManager.Domain;

namespace ContactManager.AppLogic.Contracts
{
    public  interface IContactRepository
    {
        IList<Contact> GetAllContacts();
        void AddContact(Contact contact);
    }
}
