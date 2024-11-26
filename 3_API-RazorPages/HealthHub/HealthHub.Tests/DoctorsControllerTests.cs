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
        @"HealthHub\Controllers\api\DoctorsController.cs;HealthHub\Program.cs")]
    public class DoctorsControllerTests
    {
        private Mock<IDoctorsRepository> _doctorsRepositoryMock = null!;
        private DoctorsController _doctorsController = null!;

        [SetUp]
        public void SetUp()
        {
            _doctorsRepositoryMock = new Mock<IDoctorsRepository>();
            _doctorsController = new DoctorsController(_doctorsRepositoryMock.Object);
        }

        [MonitoredTest]
        public void _01_GetAll_ReturnsOkWithListOfDoctors()
        {
            // Arrange
            var doctors = new List<Doctor>
            {
                new Doctor { Id = 1, LastName = "Dr. Smith" },
                new Doctor { Id = 2, LastName = "Dr. Johnson" }
            };

            _doctorsRepositoryMock.Setup(repo => repo.GetAll()).Returns(doctors);

            // Act
            var result = _doctorsController.GetAll() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null, "Must return OK with data");
            _doctorsRepositoryMock.Verify(repo => repo.GetAll(), Times.Once, "The repository must be used to retrieve all doctors");
            IList<Doctor>? resultDoctors = result!.Value as IList<Doctor>;
            Assert.That(resultDoctors, Is.SameAs(doctors), "The result must contain the exact same list returned by the repository");
        }

        [MonitoredTest]
        public void _02_Get_ExistingDoctorId_ReturnsOkWithDoctor()
        {
            // Arrange         
            int doctorId = 1;
            var existingDoctor = new Doctor { Id = doctorId, LastName = "Dr. Smith" };

            _doctorsRepositoryMock.Setup(repo => repo.GetById(doctorId)).Returns(existingDoctor);

            // Act
            var result = _doctorsController.Get(doctorId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null, "Must return OK with data");
            _doctorsRepositoryMock.Verify(repo => repo.GetById(doctorId), Times.Once, "The repository is not used correctly");
            var resultDoctor = result!.Value as Doctor;
            Assert.That(resultDoctor, Is.Not.Null, "The result data should be a Doctor");
            Assert.That(resultDoctor, Is.SameAs(existingDoctor),
                "The result data should be the exact same object returned by the repository");
        }

        [MonitoredTest]
        public void _03_Get_NonExistingDoctorId_ReturnsNotFound()
        {
            // Arrange
            int nonExistingDoctorId = 99;
            _doctorsRepositoryMock.Setup(repo => repo.GetById(nonExistingDoctorId)).Returns(() => null);

            // Act
            var result = _doctorsController.Get(nonExistingDoctorId) as NotFoundResult;

            // Assert
            _doctorsRepositoryMock.Verify(repo => repo.GetById(nonExistingDoctorId), Times.Once,
                "The repository is not used correctly");
            Assert.That(result, Is.Not.Null);
        }

        [MonitoredTest]
        public void _04_Create_ValidInput_AddsNewDoctor_ReturnsCreatedAtAction()
        {
            // Arrange
            var newDoctor = new Doctor { LastName = "Dr. Anderson" };

            // Act
            var result = _doctorsController.Create(newDoctor) as CreatedAtActionResult;

            // Assert
            Assert.That(result, Is.Not.Null, "Must return CreatedAtAction result");

            var createdDoctor = result!.Value as Doctor;
            Assert.That(createdDoctor, Is.Not.Null, "Unexpected type of object in result");
            Assert.That(newDoctor.LastName, Is.EqualTo(createdDoctor!.LastName));

            _doctorsRepositoryMock.Verify(repo => repo.Add(newDoctor), Times.Once, "Repository is not used correctly");
        }

        [MonitoredTest]
        public void _05_Create_InvalidInput_ReturnsBadRequest()
        {
            // Arrange
            _doctorsController.ModelState.AddModelError("LastName", "LastName is required");
            var invalidDoctor = new Doctor { LastName = null!};

            // Act
            var result = _doctorsController.Create(invalidDoctor) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null, "Must return BadRequestObjectResult result (the data is the ModelState itself)");

            // Verify
            _doctorsRepositoryMock.Verify(repo => repo.Add(It.IsAny<Doctor>()), Times.Never, "The repository should not be used");
        }

        [MonitoredTest]
        public void _06_Update_ReturnsOk()
        {
            // Arrange
            var doctorId = 1;
            var existingDoctor = new Doctor { Id = doctorId, LastName = "Dr. Smith" };
            var updatedDoctor = new Doctor { Id = doctorId, LastName = "Dr. Smith Jr." };

            _doctorsRepositoryMock.Setup(repo => repo.GetById(existingDoctor.Id)).Returns(existingDoctor);

            // Act
            var result = _doctorsController.Update(existingDoctor.Id, updatedDoctor) as OkResult;

            // Assert
            Assert.That(result, Is.Not.Null);

            _doctorsRepositoryMock.Verify(repo => repo.GetById(updatedDoctor.Id), Times.Once,
                "The repository must be used to retrieve the existing appointment");
            _doctorsRepositoryMock.Verify(repo => repo.Update(existingDoctor), Times.Once,
                "The update method of the repository is not used correctly");
        }

        [MonitoredTest]
        public void _07_Delete_ExistingDoctor_ReturnsNoContentResult()
        {
            // Arrange
            var doctorId = 1;
            var existingDoctor = new Doctor { Id = doctorId, LastName = "Dr. Smith" };

            _doctorsRepositoryMock.Setup(repo => repo.GetById(doctorId)).Returns(existingDoctor);

            // Act
            var result = _doctorsController.Delete(doctorId) as NoContentResult;

            // Assert
            Assert.That(result, Is.Not.Null, "Must return NoContent (204)");

            // Verify
            _doctorsRepositoryMock.Verify(repo => repo.Delete(doctorId), Times.Once,
                "The delete method of the repository is not called correctly");
        }

        [MonitoredTest]
        public void _08_Delete_NonExistingDoctor_ReturnsNotFoundResult()
        {
            // Arrange
            var doctorId = 1;
            var existingDoctor = new Doctor { Id = doctorId, LastName = "Dr. Smith" };

            _doctorsRepositoryMock.Setup(repo => repo.GetById(doctorId)).Returns(() => null);

            // Act
            var result = _doctorsController.Delete(doctorId) as NotFoundResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));

            // Verify
            _doctorsRepositoryMock.Verify(repo => repo.Delete(doctorId), Times.Never);

            // Assert
            Assert.That(result, Is.Not.Null, "Must return Not Found");
            _doctorsRepositoryMock.Verify(repo => repo.Delete(It.IsAny<int>()), Times.Never,
                "The repository should not be used to delete a doctor");
        }

        [MonitoredTest]
        public void _09_GetDoctorsBySpecialty_ReturnsOkWithListOfDoctors()
        {
            // Arrange
            var doctors = new List<Doctor>
            {
                new Doctor { Id = 1, LastName = "Dr. Smith", SpecialtyId = 1 },
                new Doctor { Id = 2, LastName = "Dr. Johnson", SpecialtyId = 1 }
            };

            _doctorsRepositoryMock.Setup(repo => repo.GetDoctorsBySpecialty(1)).Returns(doctors);

            // Act
            var result = _doctorsController.GetDoctorsBySpecialty(1) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null, "Result should be OK with data");
            var resultDoctors = result!.Value as IList<Doctor>;
            Assert.That(resultDoctors, Is.Not.Null, "The result data is of an unexpected type");
            Assert.That(doctors, Is.SameAs(resultDoctors),
                "The result data should be the exact same list returned by the repository");
            _doctorsRepositoryMock.Verify(repo => repo.GetDoctorsBySpecialty(1), Times.Once,
                "The repository is not used correctly");
        }


        [MonitoredTest]
        public void _10_DoctorsController_Should_Have_RouteAttribute()
        {
            // Arrange
            var controllerType = typeof(DoctorsController);

            // Act
            var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), true)
                                               .FirstOrDefault() as RouteAttribute;

            // Assert
            Assert.That(routeAttribute, Is.Not.Null, "DoctorsController should have a Route attribute");
            Assert.That(routeAttribute!.Template, Is.EqualTo("api/[controller]"),
                "DoctorsController should have a Route attribute with the correct template");
        }


        [MonitoredTest]
        public void _11_GetAllAction_ShouldHaveHttpGetAttribute()
        {
            // Arrange
            var methodInfo = GetMethod(nameof(DoctorsController.GetAll));

            // Act
            var httpGetAttribute = methodInfo.GetCustomAttributes(typeof(HttpGetAttribute), true)
                                             .FirstOrDefault() as HttpGetAttribute;

            // Assert
            Assert.That(httpGetAttribute, Is.Not.Null, "The GetAll method should have a HttpGet attribute");
            Assert.That(httpGetAttribute!.Template, Is.Null, "The GetAll method, shouldn't have a template");
        }

        [MonitoredTest]
        public void _12_GetAction_ShouldHaveHttpGetAttribute()
        {
            // Arrange
            var methodInfo = GetMethod(nameof(DoctorsController.Get));

            // Act
            var httpGetAttribute = methodInfo.GetCustomAttributes(typeof(HttpGetAttribute), true)
                                             .FirstOrDefault() as HttpGetAttribute;

            // Assert
            Assert.That(httpGetAttribute, Is.Not.Null, "The Get method should have a HttpGet attribute");
            Assert.That(httpGetAttribute!.Template, Is.EqualTo("{id}"),
                "The Get method should have a HttpGet attribute with the correct template");
        }

        [MonitoredTest]
        public void _13_GetDoctorsBySpecialtyAction_ShouldHaveHttpGetAttribute()
        {
            // Arrange
            var methodInfo = GetMethod(nameof(DoctorsController.GetDoctorsBySpecialty));

            // Act
            var httpGetAttribute = methodInfo.GetCustomAttributes(typeof(HttpGetAttribute), true)
                                             .FirstOrDefault() as HttpGetAttribute;

            // Assert
            Assert.That(httpGetAttribute, Is.Not.Null, "The GetDoctorsBySpecialty method should have a HttpGet attribute");
            Assert.That(httpGetAttribute!.Template, Is.EqualTo("specialty/{specialtyId}"),
                "The GetDoctorsBySpecialty method should have a HttpGet attribute with the correct template");
        }

        [MonitoredTest]
        public void _14_CreateAction_ShouldHaveHttpPostAttribute()
        {
            // Arrange
            var methodInfo = GetMethod(nameof(DoctorsController.Create));

            // Act
            var httpPostAttribute = methodInfo.GetCustomAttributes(typeof(HttpPostAttribute), true)
                                             .FirstOrDefault() as HttpPostAttribute;

            // Assert
            Assert.That(httpPostAttribute, Is.Not.Null, "The Create method should have a HttpPost attribute");
        }

        [MonitoredTest]
        public void _15_UpdateAction_ShouldHaveHttpPutAttribute()
        {
            // Arrange
            Type[] typesArray = { typeof(int), typeof(Doctor) };
            var methodInfo = GetMethod(nameof(DoctorsController.Update));

            // Act
            var httpPutAttribute = methodInfo.GetCustomAttributes(typeof(HttpPutAttribute), true)
                                             .FirstOrDefault() as HttpPutAttribute;

            // Assert
            Assert.That(httpPutAttribute, Is.Not.Null, "The Update method should have a HttpPut attribute");
            Assert.That(httpPutAttribute!.Template, Is.EqualTo("{id}"),
                "The Update method should have a HttpPut attribute with the correct template");

        }

        [MonitoredTest]
        public void _16_DeleteAction_ShouldHaveHttpDeleteAttribute()
        {
            // Arrange
            var methodInfo = GetMethod(nameof(DoctorsController.Delete));

            // Act
            var httpDeleteAttribute = methodInfo.GetCustomAttributes(typeof(HttpDeleteAttribute), true)
                                             .FirstOrDefault() as HttpDeleteAttribute;

            // Assert
            Assert.That(httpDeleteAttribute, Is.Not.Null, "The Delete method should have a HttpDelete attribute");
            Assert.That(httpDeleteAttribute!.Template, Is.EqualTo("{id}"),
                "The Delete method should have a HttpDelete attribute with the correct template");

        }

        private static MethodInfo GetMethod(string methodName)
        {
            Type controllerType = typeof(DoctorsController);
            MethodInfo? methodInfo = controllerType.GetMethod(methodName);

            Assert.That(methodInfo, Is.Not.Null, $"Method with name '{methodName}' and specified parameters not found in {controllerType.Name}.");

            return methodInfo!;
        }
    }
}