using System;
using System.Collections.Generic;

namespace VetClinic.Models.Entities
{
    public class Pet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public decimal Weight { get; set; }
        public string HealthCondition { get; set; }
        public string Diagnosis { get; set; }
        public string Gender { get; set; }
        public PetOwner Owner { get; set; }
        public Species Species { get; set; }
        public List<Breed> Breeds { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            return obj is Pet pet && Id == pet.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }

    public class Species
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            return obj is Species spec && Id == spec.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }

    public class Breed
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Species Species { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            return obj is Breed breed && Id == breed.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
