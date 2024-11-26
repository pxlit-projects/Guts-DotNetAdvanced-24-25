using HealthHub.AppLogic;
using HealthHub.Controllers.api;
using HealthHub.Domain;
using HealthHub.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Reflection;
using Guts.Client.Core;

namespace HealthHub.Tests
{
    [ExerciseTestFixture("dotnet2", "4-RAZORWEBAPI", "HealthHub",
        @"HealthHub\Controllers\api\AppointmentsController.cs")]
    public class AppointmentsControllerTests
    {
        private Mock<IAppointmentsRepository> _appointmentsRepositoryMock = null!;
        private AppointmentsController _appointmentsController = null!;

        [SetUp]
        public void SetUp()
        {
            _appointmentsRepositoryMock = new Mock<IAppointmentsRepository>();
            _appointmentsController = new AppointmentsController(_appointmentsRepositoryMock.Object);

        }

        [MonitoredTest]
        public void _01_GetAll_ReturnsOkWithAppointmentsFromRepository()
        {
            // Arrange
            var allAppointments = new List<Appointment> { new Appointment() };
            _appointmentsRepositoryMock.Setup(repo => repo.GetAll()).Returns(allAppointments);

            // Act
            var result = _appointmentsController.GetAll() as OkObjectResult;

            // Assert
            Assert.That(result,Is.Not.Null, "Must return OK with data");
            IList<Appointment>? returnedAppointments = result!.Value as IList<Appointment>;
            Assert.That(returnedAppointments, Is.Not.Null, "Unexpected type of object in result");
            Assert.That(returnedAppointments, Is.SameAs(allAppointments),
                "Result does not contain the appointments returned by the repository");
        }

        [MonitoredTest]
        public void _02_Get_WithValidId_ReturnsOkWithAppointment()
        {
            // Arrange
            int appointmentId = 1;
            _appointmentsRepositoryMock.Setup(repo => repo.GetById(appointmentId)).Returns(new Appointment { Id = appointmentId });

            // Act
            var result = _appointmentsController.Get(appointmentId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null, "Must return OK with data");
            var appointment = result!.Value as Appointment;
            Assert.That(appointment, Is.Not.Null, "Unexpected type of object in result");
            Assert.That(appointmentId, Is.EqualTo(appointment!.Id), "Result does not contain the appointment returned by the repository");
        }

        [MonitoredTest]
        public void _03_Get_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var appointmentId = 1;
            _appointmentsRepositoryMock.Setup(repo => repo.GetById(appointmentId)).Returns(() => null);

            // Act
            var result = _appointmentsController.Get(appointmentId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [MonitoredTest]
        public void _04_Create_ValidInput_AddsNewAppointment_ReturnsCreatedAtAction()
        {
            // Arrange
            var newAppointment = new Appointment
            {
                AppointmentDate = DateTime.Today, DoctorId = 1,
                PatientNationalNumber = Guid.NewGuid().ToString(), Reason = Guid.NewGuid().ToString()
            };

            // Act
            var result = _appointmentsController.Create(newAppointment) as CreatedAtActionResult;

            // Assert
            Assert.That(result, Is.Not.Null, "Must return CreatedAtAction result");

            var createdAppointment = result!.Value as Appointment;
            Assert.That(createdAppointment, Is.Not.Null, "Unexpected type of object in result");
            _appointmentsRepositoryMock.Verify(repo => repo.Add(newAppointment), Times.Once, "Repository is not used correctly");
            Assert.That(newAppointment.AppointmentDate, Is.EqualTo(createdAppointment!.AppointmentDate));
            Assert.That(newAppointment.PatientNationalNumber, Is.EqualTo(createdAppointment.PatientNationalNumber));
            Assert.That(newAppointment.DoctorId, Is.EqualTo(createdAppointment.DoctorId));
            Assert.That(newAppointment.Reason, Is.EqualTo(createdAppointment.Reason));  

            // Verify
            _appointmentsRepositoryMock.Verify(repo => repo.Add(newAppointment), Times.Once);
        }

        [MonitoredTest]
        public void _05_Create_InvalidInput_ReturnsBadRequest()
        {
            // Arrange
            _appointmentsController.ModelState.AddModelError("AppointmentDate", "AppointmentDate is required");

            var invalidAppointment = new Appointment { Id = 3 };

            // Act
            var result = _appointmentsController.Create(invalidAppointment) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null, "Must return BadRequestObjectResult result (the data is the ModelState itself)");

            // Verify
            _appointmentsRepositoryMock.Verify(repo => repo.Add(It.IsAny<Appointment>()), Times.Never, "The repository should not be used");
        }

        [MonitoredTest]
        public void _06_Update_ReturnsOk()
        {
            // Arrange
            var appointmentId = 1;
            var existingAppointment = new Appointment { Id = appointmentId, AppointmentDate = DateTime.Today };
            var updatedAppointment = new Appointment { Id = appointmentId, AppointmentDate = DateTime.Today.AddDays(10)};

            _appointmentsRepositoryMock.Setup(repo => repo.GetById(existingAppointment.Id)).Returns(existingAppointment);

            // Act
            var result = _appointmentsController.Update(existingAppointment.Id, updatedAppointment) as OkResult;

            // Assert
            Assert.That(result, Is.Not.Null);

            // Verify
            _appointmentsRepositoryMock.Verify(repo => repo.GetById(updatedAppointment.Id), Times.Once,
                "The repository must be used to retrieve the existing appointment");
            _appointmentsRepositoryMock.Verify(repo => repo.Update(existingAppointment), Times.Once,
                "The update method of the repository is not used correctly");
        }

        [MonitoredTest]
        public void _07_Delete_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var invalidAppointment = new Appointment() { Id = 999 };
            _appointmentsRepositoryMock.Setup(repo => repo.GetById(invalidAppointment.Id)).Returns(() => null);

            // Act
            var result = _appointmentsController.Delete(invalidAppointment.Id) as NotFoundResult;

            // Assert
            Assert.That(result, Is.Not.Null, "Must return Not Found");
            _appointmentsRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Appointment>()), Times.Never,
                "The repository should not be used to delete an appointment");
        }

        [MonitoredTest]
        public void _08_Delete_WithValidId_ReturnsNoContentResult()
        {
            // Arrange
            var appointmentId = 1;
            var existingAppointment = new Appointment
            {
                Id = appointmentId, AppointmentDate = DateTime.Today, DoctorId = 1,
                PatientNationalNumber = Guid.NewGuid().ToString(), Reason = Guid.NewGuid().ToString()
            };
            _appointmentsRepositoryMock.Setup(repo => repo.GetById(existingAppointment.Id)).Returns(existingAppointment);

            // Act
            var result = _appointmentsController.Delete(existingAppointment.Id) as NoContentResult;

            // Assert
            Assert.That(result, Is.Not.Null, "Must return NoContent (204)");
            _appointmentsRepositoryMock.Verify(repo => repo.Delete(existingAppointment), Times.Once,
                "The delete method of the repository is not called correctly");
        }

        [MonitoredTest]
        public void _09_AppointmentsController_Should_Have_RouteAttribute()
        {
            // Arrange
            var controllerType = typeof(AppointmentsController);

            // Act
            var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), true)
                                               .FirstOrDefault() as RouteAttribute;

            // Assert
            Assert.That(routeAttribute, Is.Not.Null, "AppointmentsController should have a Route attribute");
            Assert.That(routeAttribute!.Template, Is.EqualTo("api/[controller]"), "The controller should have a Route attribute with the correct template");
        }


        [MonitoredTest]
        public void _10_GetAllAction_ShouldHaveHttpGetAttribute()
        {
            // Arrange
            var methodInfo = GetMethod(nameof(AppointmentsController.GetAll));

            // Act
            var httpGetAttribute = methodInfo.GetCustomAttributes(typeof(HttpGetAttribute), true)
                                             .FirstOrDefault() as HttpGetAttribute;

            // Assert
            Assert.That(httpGetAttribute, Is.Not.Null, "The GetAll method of the AppointmentsController should have a HttpGet attribute");
            Assert.That(httpGetAttribute!.Template, Is.Null, "The GetAll method of the AppointmentsController, shouldn't have a template");
        }

        [MonitoredTest]
        public void _11_GetAction_ShouldHaveHttpGetAttribute()
        {
            // Arrange
            var methodInfo = GetMethod(nameof(AppointmentsController.Get));

            // Act
            var httpGetAttribute = methodInfo.GetCustomAttributes(typeof(HttpGetAttribute), true)
                                             .FirstOrDefault() as HttpGetAttribute;

            // Assert
            Assert.That(httpGetAttribute, Is.Not.Null, "The Get method of the AppointmentsController should have a HttpGet attribute");
            Assert.That(httpGetAttribute!.Template, Is.EqualTo("{id}"), "The Get method should have a HttpGet attribute with the correct template");
        }
        
        [MonitoredTest]
        public void _12_GetAppointmentsForDoctorAction_ShouldHaveHttpGetAttribute()
        {
            // Arrange
            var methodInfo = GetMethod(nameof(AppointmentsController.GetAppointmentsForDoctor));

            // Act
            var httpGetAttribute = methodInfo.GetCustomAttributes(typeof(HttpGetAttribute), true)
                                             .FirstOrDefault() as HttpGetAttribute;

            // Assert
            Assert.That(httpGetAttribute, Is.Not.Null, "The GetAppointmentsForDoctor method of the AppointmentsController should have a HttpGet attribute");
            Assert.That(httpGetAttribute!.Template, Is.EqualTo("doctor/{doctorId}"), "The Get method (with parameter) should have a HttpGet attribute with the correct template");
        }

        [MonitoredTest]
        public void _13_GetAppointmentsForPatientAction_ShouldHaveHttpGetAttribute()
        {
            // Arrange
            var methodInfo = GetMethod(nameof(AppointmentsController.GetAppointmentsForPatient));

            // Act
            var httpGetAttribute = methodInfo.GetCustomAttributes(typeof(HttpGetAttribute), true)
                                             .FirstOrDefault() as HttpGetAttribute;

            // Assert
            Assert.That(httpGetAttribute, Is.Not.Null, "The GetAppointmentsForPatient method of the AppointmentsController should have a HttpGet attribute");
            Assert.That(httpGetAttribute!.Template, Is.EqualTo("patient/{patientNationalNumber}"), "The GetAppointmentsForPatient method (with parameter) should have a HttpGet attribute with the correct template");
        }

        [MonitoredTest]
        public void _14_GetUpcomingAppointmentsAction_ShouldHaveHttpGetAttribute()
        {
            // Arrange
            var methodInfo = GetMethod(nameof(AppointmentsController.GetUpcomingAppointments));

            // Act
            var httpGetAttribute = methodInfo.GetCustomAttributes(typeof(HttpGetAttribute), true)
                                             .FirstOrDefault() as HttpGetAttribute;

            // Assert
            Assert.That(httpGetAttribute, Is.Not.Null, "The GetUpcomingAppointments method of the AppointmentsController should have a HttpGet attribute");
            Assert.That(httpGetAttribute!.Template, Is.EqualTo("upcoming"), "The GetUpcomingAppointments method (with parameter) should have a HttpGet attribute with the correct template");
        }

        [MonitoredTest]
        public void _15_CreateAction_ShouldHaveHttpPostAttribute()
        {
            // Arrange
            var methodInfo = GetMethod(nameof(AppointmentsController.Create));

            // Act
            var httpPostAttribute = methodInfo.GetCustomAttributes(typeof(HttpPostAttribute), true)
                                             .FirstOrDefault() as HttpPostAttribute;

            // Assert
            Assert.That(httpPostAttribute, Is.Not.Null, "The Create method of the DoctorsController should have a HttpPost attribute");
        }

        [MonitoredTest]
        public void _16_UpdateAction_ShouldHaveHttpPutAttribute()
        {
            // Arrange
            Type[] typesArray = { typeof(int), typeof(Appointment) };
            var methodInfo = GetMethod(nameof(AppointmentsController.Update));

            // Act
            var httpPutAttribute = methodInfo.GetCustomAttributes(typeof(HttpPutAttribute), true)
                                             .FirstOrDefault() as HttpPutAttribute;

            // Assert
            Assert.That(httpPutAttribute, Is.Not.Null, "The Put method of the appointmentsController should have a HttpPut attribute");
            Assert.That(httpPutAttribute!.Template, Is.EqualTo("{id}"), "The Update method should have a HttpPut attribute with the correct template");

        }

        [MonitoredTest]
        public void _17_DeleteAction_ShouldHaveHttpDeleteAttribute()
        {
            // Arrange
            var methodInfo = GetMethod(nameof(AppointmentsController.Delete));

            // Act
            var httpDeleteAttribute = methodInfo.GetCustomAttributes(typeof(HttpDeleteAttribute), true)
                                             .FirstOrDefault() as HttpDeleteAttribute;

            // Assert
            Assert.That(httpDeleteAttribute, Is.Not.Null, "The Delete method of the DoctorsController should have a HttpPost attribute");
            Assert.That(httpDeleteAttribute!.Template, Is.EqualTo("{id}"), "The Delete method should have a HttpDelete attribute with the correct template");

        }

        private static MethodInfo GetMethod(string methodName)
        {
            Type controllerType = typeof(AppointmentsController);
            MethodInfo? methodInfo = controllerType.GetMethod(methodName);

            Assert.That(methodInfo, Is.Not.Null, $"Method with name '{methodName}' and specified parameters not found in {controllerType.Name}.");

            return methodInfo!;
        }
    }
}
