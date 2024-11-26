using HealthHub.Domain;

namespace HealthHub.AppLogic
{
    public interface IDoctorsRepository
    {
        IList<Doctor> GetAll();
        Doctor? GetById(int id);
        void Add(Doctor doctor);
        void Update(Doctor doctor);
        void Delete(int id);
        public IList<Doctor> GetDoctorsBySpecialty(int specialtyId);
    }
}