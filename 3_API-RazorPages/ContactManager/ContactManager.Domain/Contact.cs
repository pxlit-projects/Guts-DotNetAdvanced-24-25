using System.ComponentModel.DataAnnotations;

namespace ContactManager.Domain
{
    public class Contact
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string FirstName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public int CompanyId { get; set; }
        public Company? Company { get; set; }

        public Contact()
        {
            Name=string.Empty;
            FirstName=string.Empty;
            Email=string.Empty;
            Phone=string.Empty;
        }
    }
}