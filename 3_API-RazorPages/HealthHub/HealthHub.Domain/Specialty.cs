using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthHub.Domain
{
    public class Specialty
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }
        public List<Doctor> Doctors { get; set; }

        public Specialty()
        {
            Name = string.Empty;
            Doctors = new List<Doctor>();
        }
    }
}
