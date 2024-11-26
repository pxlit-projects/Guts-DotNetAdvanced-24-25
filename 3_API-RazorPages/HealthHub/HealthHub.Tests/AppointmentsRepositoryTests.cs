using Guts.Client.Core.TestTools;
using HealthHub.AppLogic;
using HealthHub.Domain;
using HealthHub.Infrastructure;
using System;
using Guts.Client.Core;

namespace HealthHub.Tests
{
    [ExerciseTestFixture("dotnet2", "4-RAZORWEBAPI", "HealthHub",
        @"HealthHub.Infrastructure\AppointmentsRepository.cs")]
    internal class AppointmentsRepositoryTests : DataBaseTests
    {
        [MonitoredTest]
        public void _01_InterfaceShouldNotHaveBeenChanged()
        {
            var filePath = @"HealthHub.AppLogic\IAppointmentsRepository.cs";
            var fileHash = Solution.Current.GetFileHash(filePath);
            Assert.That(fileHash, Is.EqualTo("44-68-28-AF-5E-01-C5-D1-37-C8-26-1C-9A-2B-A5-5B"),
                $"The file '{filePath}' has changed. " +
                "Undo your changes on the file to make this test pass.");
        }

        [MonitoredTest]
        public void _02_ShouldImplementIAppointmentsRepository()
        {
            Type appointmentsRepositoryType = typeof(AppointmentsRepository);
            Assert.That(typeof(AppointmentsRepository).IsAssignableFrom(appointmentsRepositoryType), Is.True);
        }

        [MonitoredTest]
        public void _03_GetAll_ShouldReturnAllAppointments()
        {
            using var context = CreateDbContext(true);
            IAppointmentsRepository repository = new AppointmentsRepository(context);
            string patient1NationalNumber = Guid.NewGuid().ToString();
            var appointment1 = new Appointment()
            {
                Id = 1, DoctorId = 1, AppointmentDate = DateTime.Today.AddDays(-10), PatientNationalNumber = patient1NationalNumber,
                Reason = Guid.NewGuid().ToString()
            };
            context.Appointments.Add(appointment1);

            var patient2NationalNumber = Guid.NewGuid().ToString();
            var appointment2 = new Appointment()
            {
                Id = 2, DoctorId = 1, AppointmentDate = DateTime.Today.AddDays(-10), PatientNationalNumber = patient2NationalNumber,
                Reason = Guid.NewGuid().ToString()
            };
            context.Appointments.Add(appointment2);

            context.SaveChanges();

            IReadOnlyList<Appointment> appointments = (IReadOnlyList<Appointment>)repository.GetAll();

            Assert.That(appointments, Has.Count.EqualTo(3), "The appointment list has to contain 3 appointments.");
            Assert.That(appointments.Any(a => a.PatientNationalNumber == patient1NationalNumber), Is.True, "The appointment has the wrong PatientNationalNumber.");
            Assert.That(appointments.Any(a => a.PatientNationalNumber == patient2NationalNumber), Is.True, "The appointment has the wrong PatientNationalNumber.");
            Assert.That(appointments.Any(a => a.PatientNationalNumber == "111111"), Is.True, "The appointment has the wrong PatientNationalNumber.");
        }

        [MonitoredTest]
        public void _04_GetById_ShouldReturnTheCorrectAppointment()
        {
            using var context = CreateDbContext(true);
            IAppointmentsRepository repository = new AppointmentsRepository(context);

            string patientNationalNumber = Guid.NewGuid().ToString();
            var appointment = new Appointment()
            {
                Id = 3, DoctorId = 2, AppointmentDate = DateTime.Today.AddDays(-10), PatientNationalNumber = patientNationalNumber,
                Reason = Guid.NewGuid().ToString()
            };
            context.Appointments.Add(appointment);
            context.SaveChanges();
            Appointment? returnedAppointment = repository.GetById(3);

            Assert.That(returnedAppointment, Is.Not.Null, "The appointment is not found.");
            Assert.That(returnedAppointment!.AppointmentDate, Is.EqualTo(DateTime.Today.AddDays(-10)), "The appointment list contains a appointment for today.");
            Assert.That(returnedAppointment.Reason, Is.EqualTo(appointment.Reason), "The appointment has the wrong Reason.");
        }

        [MonitoredTest]
        public void _05_GetAppointmentsForDoctor_ShouldReturnAllAppointmentsOfDoctor()
        {
            using var context = CreateDbContext(true);
            IAppointmentsRepository repository = new AppointmentsRepository(context);

            var appointment1 = new Appointment()
            {
                Id = 1024, DoctorId = 3, AppointmentDate = DateTime.Today.AddDays(-10), PatientNationalNumber = Guid.NewGuid().ToString(),
                Reason = Guid.NewGuid().ToString()
            };
            context.Appointments.Add(appointment1);

            var appointment2 = new Appointment()
            {
                Id = 1025, DoctorId = 3, AppointmentDate = DateTime.Today.AddDays(-10), PatientNationalNumber = Guid.NewGuid().ToString(),
                Reason = Guid.NewGuid().ToString()
            };
            context.Appointments.Add(appointment2);

            context.SaveChanges();
            IEnumerable<Appointment> returnedAppointments = repository.GetAppointmentsForDoctor(3);
            Assert.That(returnedAppointments.Count, Is.EqualTo(2), "The appointment list should contain 2 appointments.");
        }

        [MonitoredTest]
        public void _06_GetAppointmentsForPatient_ShouldReturnAllAppointmentsOfPatient()
        {
            using var context = CreateDbContext(true);
            IAppointmentsRepository repository = new AppointmentsRepository(context);

            string nationalNumber = Guid.NewGuid().ToString();
            var appointment1 = new Appointment()
            {
                Id = 5024, DoctorId = 4, AppointmentDate = DateTime.Today.AddDays(-10), PatientNationalNumber = nationalNumber,
                Reason = Guid.NewGuid().ToString()
            };
            context.Appointments.Add(appointment1);

            var appointment2 = new Appointment()
            {
                Id = 5025, DoctorId = 4, AppointmentDate = DateTime.Today.AddDays(-10), PatientNationalNumber = nationalNumber,
                Reason = Guid.NewGuid().ToString()
            };
            context.Appointments.Add(appointment2);

            context.SaveChanges();

            IEnumerable<Appointment> returnedAppointments = repository.GetAppointmentsForPatient(nationalNumber);

            Assert.That(returnedAppointments.Count, Is.EqualTo(2), "The appointment list should contain 2 appointments.");
        }

        [MonitoredTest]
        public void _07_GetUpcomingAppointments_ShouldReturnAllUpcomingAppointments()
        {
            using var context = CreateDbContext(true);
            IAppointmentsRepository repository = new AppointmentsRepository(context);

            var guid = Guid.NewGuid().ToString();
            var appointment1 = new Appointment()
            {
                Id = 2024, DoctorId = 5, AppointmentDate = DateTime.Today.AddDays(10), PatientNationalNumber = guid,
                Reason = guid
            };
            context.Appointments.Add(appointment1);

            var guid2 = Guid.NewGuid().ToString();
            var appointment2 = new Appointment()
            {
                Id = 2025, DoctorId = 5, AppointmentDate = DateTime.Today.AddDays(10), PatientNationalNumber = guid,
                Reason = guid2
            };
            context.Appointments.Add(appointment2);

            context.SaveChanges();
            IList<Appointment> returnedAppointments = repository.GetUpcomingAppointments(10);

            Assert.That(returnedAppointments.Count, Is.EqualTo(2), "The appointment list should contain 2 appointments.");
        }

        [MonitoredTest]
        public void _08_Add_ShouldAddAnAppointment()
        {
            int appointmentId = Random.Shared.Next(100, int.MaxValue);
            var newAppointment = new Appointment()
            {
                Id = appointmentId,
                AppointmentDate = DateTime.Today,
                DoctorId = 6,
                PatientNationalNumber = Guid.NewGuid().ToString(),
                Reason = Guid.NewGuid().ToString()
            };

            using (var actContext = CreateDbContext(true))
            {
                var repository = new AppointmentsRepository(actContext);
                repository.Add(newAppointment);
            }

            using (var assertContext = CreateDbContext(true))
            {
                Appointment? addedAppointment = assertContext.Appointments.FirstOrDefault(a => a.Id == appointmentId);
                Assert.That(addedAppointment, Is.Not.Null, "The appointment is not added correctly in the database");
            }
        }

        [MonitoredTest]
        public void _09_Update_ShouldUpdateAppointment()
        {
            //Arrange
            Appointment appointment = null!;
            int appointmentId = Random.Shared.Next(100, int.MaxValue);
            int originalNumberOfAppointments = 0;
            using (var arrangeContext = CreateDbContext(true))
            {
                appointment = new Appointment()
                {
                    Id = appointmentId,
                    AppointmentDate = DateTime.Today,
                    DoctorId = 7,
                    PatientNationalNumber = Guid.NewGuid().ToString(),
                    Reason = Guid.NewGuid().ToString()
                };
                arrangeContext.Add(appointment);
                arrangeContext.SaveChanges();
                originalNumberOfAppointments = arrangeContext.Appointments.Count();
            }

            //Act
            string newPatientNationalNumber = Guid.NewGuid().ToString();
            using (var actContext = CreateDbContext(false))
            {
                IAppointmentsRepository repository = new AppointmentsRepository(actContext);
                appointment.PatientNationalNumber = newPatientNationalNumber;
                repository.Update(appointment);
            }

            //Assert
            using (var assertContext = CreateDbContext(false))
            {
                Appointment? updatedAppointment = assertContext.Set<Appointment>().FirstOrDefault(a => a.Id == appointmentId);
                Assert.That(updatedAppointment, Is.Not.Null);
                int numberOfAppointments = assertContext.Appointments.Count();
                Assert.That(numberOfAppointments, Is.EqualTo(originalNumberOfAppointments),
                    "An appointment was added or deleted instead of updated");
                Assert.That(updatedAppointment!.PatientNationalNumber, Is.EqualTo(newPatientNationalNumber),
                    "The appointment is not updated correctly");
            }
        }

        [MonitoredTest]
        public void _10_Delete_ShouldDeleteAppointment()
        {
            Appointment appointmentToDelete = null!;
            using (var actContext = CreateDbContext(true))
            {
                IAppointmentsRepository repository = new AppointmentsRepository(actContext);

                appointmentToDelete = actContext.Appointments.First();
                repository.Delete(appointmentToDelete);
            }

            using (var assertContext = CreateDbContext(true))
            {
                Appointment? deletedAppointment = assertContext.Appointments.FirstOrDefault(a => a.Id == appointmentToDelete.Id);

                Assert.That(deletedAppointment, Is.Null, "The appointment is not deleted in the database.");
            }
        }

    }
}
