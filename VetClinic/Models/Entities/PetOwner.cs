using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;

namespace VetClinic.Models.Entities
{
    public class PetOwner
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }

        public override string ToString()
        {
            return FullName;
        }

        public override bool Equals(object obj)
        {
            return obj is PetOwner owner && Id == owner.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
