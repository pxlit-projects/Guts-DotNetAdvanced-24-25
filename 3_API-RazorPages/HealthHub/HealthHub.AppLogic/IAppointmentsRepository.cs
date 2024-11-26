using HealthHub.Domain;

namespace HealthHub.AppLogic
{
    public interface IAppointmentsRepository
    {
        IList<Appointment> GetAll();
        Appointment? GetById(int id);
        void Add(Appointment appointment);
        void Update(Appointment appointment);
        void Delete(Appointment appointment);
        IList<Appointment> GetAppointmentsForDoctor(int doctorId);
        IList<Appointment> GetUpcomingAppointments(int daysAhead);
        IList<Appointment> GetAppointmentsForPatient(string patientNationalNumber);
    }
}
