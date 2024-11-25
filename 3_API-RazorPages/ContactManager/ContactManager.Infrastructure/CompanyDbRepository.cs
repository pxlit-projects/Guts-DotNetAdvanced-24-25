using ContactManager.AppLogic.Contracts;
using ContactManager.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactManager.Infrastructure
{
    internal class CompanyDbRepository : ICompanyRepository
    {
        public void AddCompany(Company company)
        {
            throw new NotImplementedException();
        }

        public IList<Company> GetAllCompanies()
        {
            throw new NotImplementedException();
        }
    }
}
