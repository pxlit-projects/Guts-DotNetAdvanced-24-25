using HealthHub.Domain;
using Microsoft.EntityFrameworkCore;

namespace HealthHub.Infrastructure
{
    public class HealthHubDbContext : DbContext
    {
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        public HealthHubDbContext(DbContextOptions<HealthHubDbContext> options) : base(options)
        {
        }

        private IList<Specialty> GetSpecialties()
        {
            return new List<Specialty>()
            {
                new Specialty() {Id = 1, Name = "Cardiology"},
                new Specialty() {Id = 2, Name = "Dermatology"},
                new Specialty() {Id = 3, Name = "Orthopedics"},
                new Specialty() {Id = 4, Name = "Neurology"},
                new Specialty() {Id = 5, Name = "Pediatrics"}            
            };
        }
        private IList<Doctor> GetDoctors()
        {
            return new List<Doctor>()
            {
                new Doctor() {Id = 1, FirstName = "Sophie", LastName="Van Damme", Email="dr.sophie.vandamme@example.com", Phone="+32 479 12 34 56", SpecialtyId=1},
                new Doctor() {Id = 2, FirstName = "Thomas", LastName = "De Vos", Email="dr.Thomas.Devox@example.com", Phone="+32 473 98 76 54", SpecialtyId = 1},
                new Doctor() {Id = 3, FirstName = "Marie", LastName = "Dubois", Email="dr.Marie.Dubois@example.com", Phone="+32 488 11 11 11", SpecialtyId = 2},
                new Doctor() {Id = 4, FirstName = "Axl", LastName = "Moreau", Email = "dr.Axl.Moreau@example.be", Phone="+32 488 22 22 22", SpecialtyId = 3 },
                new Doctor() {Id = 5, FirstName = "Peter", LastName = "McHealer", Email = "dr.Peter.Mchealer@example.be", Phone="+32 499 33 33 33", SpecialtyId = 3 },
                new Doctor() {Id = 6, FirstName = "Kate", LastName = "Grant", Email = "dr.Kate.Grant@example.be", Phone="+32 473 55 55 55", SpecialtyId = 3 },
                new Doctor() {Id = 7, FirstName = "Simon", LastName = "De Jong", Email = "dr.Simon.DeJong@example.be", Phone="+32 474 66 66 22", SpecialtyId = 4 },
                new Doctor() {Id = 8, FirstName = "Bryan", LastName = "De Vries", Email = "dr.Bryan.Devries@example.be", Phone="+32 475 77 77 77", SpecialtyId = 5 }

            };
        }

        private IList<Appointment> GetAppointments()
        {
            return new List<Appointment>()
            { 
                new Appointment() {Id = 9999,DoctorId=1, AppointmentDate=DateTime.Today, PatientNationalNumber="111111", Reason="reason"}
            };
        }

    }
}

