
using ContactManager.Domain;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Infrastructure
{
    public class ContactManagerDbContext : DbContext
    {
        public ContactManagerDbContext(DbContextOptions<ContactManagerDbContext> options) : base(options)
        {
        }

        private IList<Company> GetCompanies()
        {
            return new List<Company>() {
                new Company()
                {
                    Id = 1,
                    Name = "TechCo",
                    Address = "123 Main Street",
                    Zip = "12345",
                    City = "Techville"
                },
                new Company
                {
                    Id = 2,
                    Name = "Widgets Inc.",
                    Address = "456 Elm Street",
                    Zip = "67890",
                    City = "Widgetville"
                } };
        }

        private IList<Contact> GetContacts()
        {
            return new List<Contact>() {
                    new Contact() { Id= 1, Name = "Doe", FirstName = "John", Email = "john@example.com", Phone = "555-1234", CompanyId = 1 },
                    new Contact() { Id=2, Name = "Smith", FirstName = "Jane", Email = "jane@example.com", Phone = "555-5678", CompanyId = 1 },
                    new Contact() { Id=3, Name = "Johnson", FirstName = "Alice", Email = "alice@example.com", Phone = "555-4321", CompanyId = 2 }
                };
        }
    }
}