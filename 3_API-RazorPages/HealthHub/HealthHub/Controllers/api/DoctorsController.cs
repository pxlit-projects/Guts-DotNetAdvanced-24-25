using HealthHub.AppLogic;
using HealthHub.Domain;
using HealthHub.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace HealthHub.Controllers.api
{
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        public DoctorsController(IDoctorsRepository doctorRepository)
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

        public IActionResult Create([FromBody] Doctor doctor)
        {
            throw new NotImplementedException();
        }

        public IActionResult Update(int id, [FromBody] Doctor updatedDoctor)
        {
            throw new NotImplementedException();
        }

        public IActionResult Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IActionResult GetDoctorsBySpecialty(int specialtyId)
        {
            throw new NotImplementedException();
        }
    }
}
