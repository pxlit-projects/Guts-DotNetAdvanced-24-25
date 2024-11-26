using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthHub.Domain
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        
        public Doctor? Doctor { get; set; }
        public int DoctorId { get; set; }

        [MaxLength(100)]
        public string PatientNationalNumber { get; set; }

        [MaxLength(150)]
        public string Reason { get; set; }

        public Appointment()
        {
            PatientNationalNumber = string.Empty;
            Reason = string.Empty;
        }
    }


}
