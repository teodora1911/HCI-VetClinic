using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetClinic.Models.Entities
{
    public class Prescription
    {
        public int Examination { get; set; }
        public Medicine Medicine { get; set; }
        public string Name { get; set; }
        public int Dose { get; set; }
        public string Frequency { get; set; }
        public DateTime? Start { get; set; }
        public string Duration { get; set; }
        public string Instructions { get; set; }
    }
}
