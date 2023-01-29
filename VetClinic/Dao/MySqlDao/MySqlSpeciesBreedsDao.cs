using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.Models.Entities;
using VetClinic.Utils;

namespace VetClinic.Dao.MySqlDao
{
    public class MySqlSpeciesBreedsDao : ISpeciesBreedsDao
    {
        MySqlConnection? Connection;
        MySqlCommand? Command;
        MySqlDataReader? Reader;

        private static readonly string SelectAll_Species = "SELECT * FROM species";
        private static readonly string GetAllBreeds_SpeciesJoined = "SELECT breed.id AS breed_id, breed.name AS breed_name, species.id AS species_id, species.name AS species_name FROM breed INNER JOIN species ON breed.species=species.id";
        private static readonly string SelectBreedBySpecies = "SELECT * FROM breed WHERE species=@species";

        public List<Breed> GetAllBreeds()
        {
            Connection = null;
            Command = null;
            Reader = null;

            List<Breed> list = new();
            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = GetAllBreeds_SpeciesJoined;
                    Reader = Command.ExecuteReader();

                    while (Reader.Read())
                    {
                        list.Add(new Breed()
                        {
                            Id = Reader.GetInt32("breed_id"),
                            Name = Reader.GetString("breed_name"),
                            Species = new Species()
                            {
                                Id = Reader.GetInt32("species_id"),
                                Name = Reader.GetString("species_name")
                            }
                        });
                    }
                }
            }
            catch (Exception) { }

            return list;
        }

        public List<Species> GetAllSpecies()
        {
            Connection = null;
            Command = null;
            Reader = null;

            List<Species> list = new();
            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = SelectAll_Species;
                    Reader = Command.ExecuteReader();

                    while (Reader.Read())
                    {
                        list.Add(new Species()
                        {
                            Id = Reader.GetInt32("id"),
                            Name = Reader.GetString("name")
                        });
                    }
                }
            }
            catch (Exception) { /*TODO*/}

            return list;
        }

        public Breed GetBreedById(int id)
        {
            return GetAllBreeds().Where(b => b.Id == id).FirstOrDefault();
        }

        public List<Breed> GetBreedsBySpecies(Species species)
        {
            Connection = null;
            Command = null;
            Reader = null;

            List<Breed> list = new();
            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = SelectBreedBySpecies;
                    Command.Parameters.AddWithValue("@species", species.Id);
                    Reader = Command.ExecuteReader();

                    while (Reader.Read())
                    {
                        list.Add(new Breed()
                        {
                            Id = Reader.GetInt32("id"),
                            Name = Reader.GetString("name"),
                            Species = species
                        });
                    }
                }
            }
            catch (Exception) { }

            return list;
        }

        public Species GetSpeciesById(int id)
        {
            return GetAllSpecies().Where(s => s.Id == id).FirstOrDefault();
        }
    }
}
