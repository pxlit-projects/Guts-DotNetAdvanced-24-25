using ContactManager.AppLogic.Contracts;
using ContactManager.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ContactManager.Pages.Contacts
{
    public class AddContactModel
    {
        public AddContactModel(ICompanyRepository companyRepository, IContactRepository contactRepository)
        {
        }

        public void OnGet()
        {
            throw new NotImplementedException();
        }

        public IActionResult OnPost()
        {
            throw new NotImplementedException();
        }
    }
}
