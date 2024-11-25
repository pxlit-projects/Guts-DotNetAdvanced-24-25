using Microsoft.AspNetCore.Mvc.RazorPages;
using ContactManager.Domain;
using Microsoft.AspNetCore.Mvc;
using ContactManager.AppLogic.Contracts;


namespace ContactManager.Pages
{
    public class IndexModel
    {
        public IList<Contact> Contacts { get; set; }

        public IndexModel(IContactRepository contactRepository)
        {
        }

        public void OnGet()
        {
            throw new NotImplementedException();
        }
    }
}