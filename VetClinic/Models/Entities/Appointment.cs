using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VetClinic.Models.Entities
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Reason { get; set; }
        public bool IsScheduled { get; set; }
        public Pet Pet { get; set; }
        public Veterinarian Vet { get; set; }
    }
}
