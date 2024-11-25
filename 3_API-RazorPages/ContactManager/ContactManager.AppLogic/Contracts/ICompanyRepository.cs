using ContactManager.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.AppLogic.Contracts
{
    public interface ICompanyRepository
    {
        void AddCompany(Company company);
        IList<Company> GetAllCompanies();
    }
}
