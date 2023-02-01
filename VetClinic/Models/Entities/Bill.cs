using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetClinic.Models.Entities
{
    public class Bill
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public DateTime Timestamp { get; set; }
        public string Payment { get; set; }
        public Examination Examination { get; set; }
        public PetOwner Owner { get; set; }
    }
}
