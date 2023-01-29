using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.Models.Entities;

namespace VetClinic.Dao
{
    public interface IPetDao : IGenericSearchDao<Pet, int>
    {
        List<Species> GetAllSpecies();
        List<Breed> GetBreedsFromSpecies(Species species);
        List<Breed> GetAllBreeds();
        List<Pet> GetAllPetsOfOwner(string owner);
        List<Pet> GetAllPetsOfOwner(int id);
        List<Pet> GetAllPetsBySpecs(int ownerId, string name, Species? species, Breed? breed);
    }
}
