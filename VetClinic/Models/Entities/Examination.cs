using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetClinic.Models.Entities
{
    public class Examination
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Address { get; set; }
        public bool IsCompleted { get; set; }

        public Veterinarian Vet { get; set; }
        public Pet Pet { get; set; }
        public Appointment Appointment { get; set; }
    }

    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public string Description { get; set; }
    }

    public class ExaminationService
    {
        public int Examination { get; set; }
        public Service Service { get; set; }
        public int Quantity { get; set; }
        public decimal Cost { get; set; }
    }
}
