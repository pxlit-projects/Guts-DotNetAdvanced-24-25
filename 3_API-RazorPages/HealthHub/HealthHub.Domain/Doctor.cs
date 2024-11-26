using System.ComponentModel.DataAnnotations;

namespace HealthHub.Domain
{
    public class Doctor
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(100)]
        public string Phone { get; set; }
        public int SpecialtyId { get; set; } 

        public Doctor()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
        }
    }

}