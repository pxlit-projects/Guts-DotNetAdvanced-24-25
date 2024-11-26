using HealthHub.AppLogic;
using HealthHub.Domain;
using HealthHub.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthHub.Controllers.api
{
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        public AppointmentsController(IAppointmentsRepository appointmentsRepository)
        {
        }

        public IActionResult GetAll()
        {
            throw new NotImplementedException();
        }

        public IActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IActionResult Create([FromBody] Appointment appointment)
        {
            throw new NotImplementedException();
        }

        public IActionResult Update(int id, [FromBody] Appointment updatedAppointment)
        {
            throw new NotImplementedException();
        }

        public IActionResult Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IActionResult GetAppointmentsForDoctor(int doctorId)
        {
            throw new NotImplementedException();
        }

        public IActionResult GetAppointmentsForPatient(string patientNationalNumber)
        {
            throw new NotImplementedException();
        }

        public IActionResult GetUpcomingAppointments([FromQuery] int daysAhead)
        {
            throw new NotImplementedException();
        }
    }
}
