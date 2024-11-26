using HealthHub.AppLogic;
using HealthHub.Domain;

namespace HealthHub.Infrastructure
{
    internal class AppointmentsRepository: IAppointmentsRepository
    {
        public AppointmentsRepository(HealthHubDbContext context)
        {
        }

        public IList<Appointment> GetAll()
        {
            throw new NotImplementedException();
        }

        public Appointment? GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Add(Appointment appointment)
        {
            throw new NotImplementedException();
        }

        public void Update(Appointment appointment)
        {
            throw new NotImplementedException();
        }

        public void Delete(Appointment appointment)
        {
            throw new NotImplementedException();
        }

        public IList<Appointment> GetAppointmentsForDoctor(int doctorId)
        {
            throw new NotImplementedException();
        }

        public IList<Appointment> GetAppointmentsForPatient(string patientNationalNumber)
        {
            throw new NotImplementedException();
        }

        public IList<Appointment> GetUpcomingAppointments(int daysAhead)
        {
            throw new NotImplementedException();
        }
    }
}

