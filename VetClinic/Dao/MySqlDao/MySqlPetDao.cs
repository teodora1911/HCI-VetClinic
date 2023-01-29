using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.Models.Entities;
using VetClinic.Utils;

namespace VetClinic.Dao.MySqlDao
{
    public class MySqlPetDao : IPetDao
    {
        MySqlConnection? Connection;
        MySqlCommand? Command;
        MySqlDataReader? Reader;

        private IPetOwnerDao PetOwnerDao = DaoFactory.Instance(DaoType.MySql).PetOwners;
        private ISpeciesBreedsDao SpeciesBreedsDao = DaoFactory.Instance(DaoType.MySql).SpeciesAndBreeds;

        private static readonly string SelectAllPets = "SELECT * FROM pet";
        private static readonly string SelectPetById = SelectAllPets + " WHERE id=@id";
        private static readonly string SelectPetByOwnerId = SelectAllPets + " WHERE owner=@owner";
        private static readonly string SearchByName = SelectAllPets + " WHERE name like @name";

        //        private static readonly string GetAllBreeds_SpeciesJoined = "SELECT breed.id AS breed_id, breed.name AS breed_name, species.id AS species_id, species.name AS species_name FROM breed INNER JOIN species ON breed.species=species.id";
        //        private static readonly string GetAllPets_SpeciesJoined_OwnerJoined = "SELECT p.id AS pet_id, p.name AS pet_name, p.birthdate AS pet_birth, p.estimatedage AS pet_age, p.weight AS pet_weight, p.healthcondition AS pet_health, p.diagnosis AS pet_diagnosis, o.id AS owner_id, o.fullname AS owner_name, o.email AS owner_email, o.contactnumber AS owner_contact, s.id AS species_id, s.name AS species_name  FROM pet p INNER JOIN petowner o ON o.id=p.owner INNER JOIN species s ON s.id=p.species";
        //        private static readonly string GetAllBreedsOfPet = "SELECT b.id AS breed_id, b.name AS breed_name FROM mixed m INNER JOIN breed b ON b.id=m.breed WHERE m.pet=@id";

        private static readonly string GetMixedOfPet = "SELECT * FROM mixed WHERE pet=@pet";
        private static readonly string InsertPet = "INSERT INTO pet(name, age, weight, healthcondition, diagnosis, owner, gender, species) VALUES(@name, @age, @weight, @health, @diagnosis, @owner, @gender, @species)";
        private static readonly string InsertPetBreed = "INSERT INTO mixed(pet, breed) VALUES(@pet, @breed)";
        private static readonly string InsertBreed = "INSERT INTO breed(name, species) VALUES(@name, @species)";
        private static readonly string InsertSpecies = "INSERT INTO species(name) VALUES(@name)";

        private static readonly string UpdateById = "UPDATE pet set name=@name, age=@age, weight=@weight, healthcondition=@health, diagnosis=@diagnosis WHERE id=@id";
        private static readonly string Delete = "DELETE FROM pet WHERE id=@id";

        public int Create(Pet entity)
        {
            Connection = null;
            Command = null;
            Reader = null;

            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    // Look for Species - if is new, then add new row in species table
                    if(entity.Species.Id == 0)
                    {
                        Command = Connection.CreateCommand();
                        Command.CommandText = InsertSpecies;
                        Command.Parameters.AddWithValue("@name", entity.Species.Name);
                        Command.ExecuteNonQuery();
                        entity.Species.Id = (int)Command.LastInsertedId;
                    }

                    // Look for Breeds - if some breed doesn't have id, add new row in breed table
                    foreach(Breed breed in entity.Breeds)
                    {
                        if(breed.Id == 0)
                        {
                            Command = Connection.CreateCommand();
                            Command.CommandText = InsertBreed;
                            Command.Parameters.Clear();
                            Command.Parameters.AddWithValue("@name", breed.Name);
                            Command.Parameters.AddWithValue("@species", entity.Species.Id);
                            Command.ExecuteNonQuery();
                            breed.Id = (int)Command.LastInsertedId;
                        }
                    }

                    // Insert Values into Pet Table
                    Command = Connection.CreateCommand();
                    Command.CommandText = InsertPet;
                    Command.Parameters.AddWithValue("@name", entity.Name);
                    Command.Parameters.AddWithValue("@age", entity.Age);
                    Command.Parameters.AddWithValue("@weight", entity.Weight);
                    Command.Parameters.AddWithValue("@health", entity.HealthCondition);
                    Command.Parameters.AddWithValue("@diagnosis", entity.Diagnosis);
                    Command.Parameters.AddWithValue("@owner", entity.Owner.Id);
                    Command.Parameters.AddWithValue("@gender", entity.Gender);
                    Command.Parameters.AddWithValue("@species", entity.Species.Id);
                    Command.ExecuteNonQuery();
                    entity.Id = (int)Command.LastInsertedId;

                    // Insert Values into mixed Table
                    foreach(Breed breed in entity.Breeds)
                    {
                        Command = Connection.CreateCommand();
                        Command.CommandText = InsertPetBreed;
                        Command.Parameters.Clear();
                        Command.Parameters.AddWithValue("@pet", entity.Id);
                        Command.Parameters.AddWithValue("@breed", breed.Id);
                        Command.ExecuteNonQuery();
                    }

                    return entity.Id;
                }
            }
            catch (Exception)
            {
                // TODO
                return -1;
            }
        }

        public bool DeleteById(int id)
        {
            Connection = null;
            Command = null;
            Reader = null;

            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = Delete;
                    Command.Parameters.AddWithValue("@id", id);

                    return Command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception) { return false; }
        }

        public List<Pet> GetAll()
        {
            Connection = null;
            Command = null;
            Reader = null;

            List<Pet> list = new();
            try
            {
                using(Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = SelectAllPets;
                    Reader = Command.ExecuteReader();

                    while (Reader.Read())
                    {
                        list.Add(new Pet()
                        {
                            Id = Reader.GetInt32("id"),
                            Name = Reader.GetString("name"),
                            Age = Reader.GetInt32("age"),
                            Weight = Reader.GetDecimal("weight"),
                            HealthCondition = Reader.GetString("healthcondition"),
                            Diagnosis = Reader.GetString("diagnosis"),
                            Gender = Reader.GetString("gender"),
                            Owner = PetOwnerDao.GetById(Reader.GetInt32("owner")),
                            Species = SpeciesBreedsDao.GetSpeciesById(Reader.GetInt32("species")),
                            Breeds = new()
                        });
                    }

                    Reader.Close();
                    Reader = null;

                    // Get All Breeds Of Pet
                    foreach(Pet pet in list)
                    {
                        Command = Connection.CreateCommand();
                        Command.CommandText = GetMixedOfPet;
                        Command.Parameters.AddWithValue("@pet", pet.Id);
                        Reader = Command.ExecuteReader();

                        while(Reader.Read())
                        {
                            pet.Breeds.Add(SpeciesBreedsDao.GetBreedById(Reader.GetInt32("breed")));
                        }

                        Reader.Close();
                        Reader = null;
                    }
                }
            }
            catch (Exception) { }

            return list;
        }

        public List<Breed> GetAllBreeds() => SpeciesBreedsDao.GetAllBreeds();

        public List<Pet> GetAllPetsOfOwner(string owner) => GetAll().Where(pet => pet.Owner.FullName.ToLower().Contains(owner.ToLower())).ToList();

        public List<Pet> GetAllPetsOfOwner(int id)
        {
            Connection = null;
            Command = null;
            Reader = null;

            List<Pet> list = new();
            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = SelectPetByOwnerId;
                    Command.Parameters.AddWithValue("@owner", id);
                    Reader = Command.ExecuteReader();

                    while (Reader.Read())
                    {
                        list.Add(new Pet()
                        {
                            Id = Reader.GetInt32("id"),
                            Name = Reader.GetString("name"),
                            Age = Reader.GetInt32("age"),
                            Weight = Reader.GetDecimal("weight"),
                            HealthCondition = Reader.GetString("healthcondition"),
                            Diagnosis = Reader.GetString("diagnosis"),
                            Gender = Reader.GetString("gender"),
                            Owner = PetOwnerDao.GetById(Reader.GetInt32("owner")),
                            Species = SpeciesBreedsDao.GetSpeciesById(Reader.GetInt32("species")),
                            Breeds = new()
                        });
                    }

                    Reader.Close();
                    Reader = null;

                    // Get All Breeds Of Pet
                    foreach (Pet pet in list)
                    {
                        Command = Connection.CreateCommand();
                        Command.CommandText = GetMixedOfPet;
                        Command.Parameters.AddWithValue("@pet", pet.Id);
                        Reader = Command.ExecuteReader();

                        while (Reader.Read())
                        {
                            pet.Breeds.Add(SpeciesBreedsDao.GetBreedById(Reader.GetInt32("breed")));
                        }

                        Reader.Close();
                        Reader = null;
                    }
                }
            }
            catch (Exception) { }

            return list;
        }

        public List<Species> GetAllSpecies() => SpeciesBreedsDao.GetAllSpecies();

        public List<Breed> GetBreedsFromSpecies(Species species) => SpeciesBreedsDao.GetBreedsBySpecies(species);

        public Pet GetById(int id)
        {
            Connection = null;
            Command = null;
            Reader = null;

            Pet pet = null;
            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = SelectPetById;
                    Command.Parameters.AddWithValue("@id", id);
                    Reader = Command.ExecuteReader();

                    if (Reader.Read())
                    {
                        pet = new Pet()
                        {
                            Id = Reader.GetInt32("id"),
                            Name = Reader.GetString("name"),
                            Age = Reader.GetInt32("age"),
                            Weight = Reader.GetDecimal("weight"),
                            HealthCondition = Reader.GetString("healthcondition"),
                            Diagnosis = Reader.GetString("diagnosis"),
                            Gender = Reader.GetString("gender"),
                            Owner = PetOwnerDao.GetById(Reader.GetInt32("owner")),
                            Species = SpeciesBreedsDao.GetSpeciesById(Reader.GetInt32("species")),
                            Breeds = new()
                        };
                    }

                    Reader.Close();
                    Reader = null;

                    // Get All Breeds Of Pet
                    if(pet is not null)
                    {
                        Command = Connection.CreateCommand();
                        Command.CommandText = GetMixedOfPet;
                        Command.Parameters.AddWithValue("@pet", pet.Id);
                        Reader = Command.ExecuteReader();

                        while (Reader.Read())
                        {
                            pet.Breeds.Add(SpeciesBreedsDao.GetBreedById(Reader.GetInt32("breed")));
                        }

                        Reader.Close();
                        Reader = null;
                    }
                }
            }
            catch (Exception) { }

            return pet;
        }

        public List<Pet> GetBySearchQuery(string query)
        {
            Connection = null;
            Command = null;
            Reader = null;

            List<Pet> list = new();
            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = SearchByName;
                    Command.Parameters.AddWithValue("@name", "%" + query + "%");
                    Reader = Command.ExecuteReader();

                    while (Reader.Read())
                    {
                        list.Add(new Pet()
                        {
                            Id = Reader.GetInt32("id"),
                            Name = Reader.GetString("name"),
                            Age = Reader.GetInt32("age"),
                            Weight = Reader.GetDecimal("weight"),
                            HealthCondition = Reader.GetString("healthcondition"),
                            Diagnosis = Reader.GetString("diagnosis"),
                            Gender = Reader.GetString("gender"),
                            Owner = PetOwnerDao.GetById(Reader.GetInt32("owner")),
                            Species = SpeciesBreedsDao.GetSpeciesById(Reader.GetInt32("species")),
                            Breeds = new()
                        });
                    }

                    Reader.Close();
                    Reader = null;

                    // Get All Breeds Of Pet
                    foreach (Pet pet in list)
                    {
                        Command = Connection.CreateCommand();
                        Command.CommandText = GetMixedOfPet;
                        Command.Parameters.AddWithValue("@pet", pet.Id);
                        Reader = Command.ExecuteReader();

                        while (Reader.Read())
                        {
                            pet.Breeds.Add(SpeciesBreedsDao.GetBreedById(Reader.GetInt32("breed")));
                        }

                        Reader.Close();
                        Reader = null;
                    }
                }
            }
            catch (Exception) { }

            return list;
        }

        public bool Update(Pet entity)
        {
            Connection = null;
            Command = null;
            Reader = null;

            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = UpdateById;
                    Command.Parameters.AddWithValue("@id", entity.Id);
                    Command.Parameters.AddWithValue("@name", entity.Name);
                    Command.Parameters.AddWithValue("@age", entity.Age);
                    Command.Parameters.AddWithValue("@weight", entity.Weight);
                    Command.Parameters.AddWithValue("@health", entity.HealthCondition);
                    Command.Parameters.AddWithValue("@diagnosis", entity.Diagnosis);

                    return Command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<Pet> GetAllPetsBySpecs(int ownerId, string name, Species? species, Breed? breed)
        {
            List<Pet> list = GetBySearchQuery(name);
            list = list.Where<Pet>(item => item.Owner.Id == ownerId).ToList();

            if(species is not null)
                list = list.Where<Pet>(item => item.Species.Equals(species)).ToList();
            if(breed is not null)
                list = list.Where<Pet>(item => item.Breeds.First<Breed>().Equals(breed)).ToList();

            return list;
        }
    }
}
