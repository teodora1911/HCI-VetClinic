using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.Models.Entities;

namespace VetClinic.Dao
{
    public interface ISpeciesBreedsDao
    {
        public List<Species> GetAllSpecies();
        public List<Breed> GetAllBreeds();
        public Species GetSpeciesById(int id);
        public Breed GetBreedById(int id);
        public List<Breed> GetBreedsBySpecies(Species species);
    }
}
