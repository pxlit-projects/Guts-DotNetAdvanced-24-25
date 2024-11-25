using ContactManager.AppLogic.Contracts;
using ContactManager.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContactManager.Pages.Companies
{
    public class AddCompanyModel
    {
        public Company Company { get; set; }

        public AddCompanyModel(ICompanyRepository companyRepository)
        {
            throw new NotImplementedException();
        }

        public IActionResult OnPost()
        {
            throw new NotImplementedException();
        }
    }
}
