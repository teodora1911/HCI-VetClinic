using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.Models.Entities;
using VetClinic.Utils;

namespace VetClinic.Dao.MySqlDao
{
    public class MySqlPetOwnerDao : IPetOwnerDao
    {
        MySqlConnection? Connection;
        MySqlCommand? Command;
        MySqlDataReader? Reader;

        private static readonly string SelectAll = "SELECT * FROM petowner";
        private static readonly string SelectById = SelectAll + " WHERE id=@id";
        private static readonly string SearchByFullName = SelectAll + " WHERE fullname LIKE @name";
        private static readonly string UpdateById = "UPDATE petowner set fullname=@name, email=@email, contactnumber=@contact WHERE id=@id";
        private static readonly string Insert = "INSERT INTO petowner(fullname, email, contactnumber) VALUES(@name, @email, @contact)";
        private static readonly string Delete = "DELETE FROM petowner WHERE id=@id";

        public int Create(PetOwner entity)
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
                    Command.CommandText = Insert;
                    Command.Parameters.AddWithValue("@name", entity.FullName);
                    Command.Parameters.AddWithValue("@email", entity.Email);
                    Command.Parameters.AddWithValue("@contact", entity.Contact);
                    Command.ExecuteNonQuery();
                    entity.Id = (int)Command.LastInsertedId;
                    return entity.Id;
                }
            }
            catch (Exception e)
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
            catch (Exception e)
            {
                // TODO
                return false;
            }
        }

        public List<PetOwner> GetAll()
        {
            Connection = null;
            Command = null;
            Reader = null;

            List<PetOwner> list = new();
            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = SelectAll;
                    Reader = Command.ExecuteReader();

                    while (Reader.Read())
                    {
                        list.Add(new PetOwner()
                        {
                            Id = Reader.GetInt32("id"),
                            FullName = Reader.GetString("fullname"),
                            Email = Reader.GetString("email"),
                            Contact = Reader.GetString("contactnumber")
                        });
                    }
                }
            }
            catch (Exception e)
            {
                // TODO
            }

            return list;
        }

        public PetOwner GetById(int id)
        {
            Connection = null;
            Command = null;
            Reader = null;

            PetOwner owner = null;
            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = SelectById;
                    Command.Parameters.AddWithValue("@id", id);
                    Reader = Command.ExecuteReader();

                    if (Reader.Read())
                    {
                        owner = new PetOwner()
                        {
                            Id = Reader.GetInt32("id"),
                            FullName = Reader.GetString("fullname"),
                            Email = Reader.GetString("email"),
                            Contact = Reader.GetString("contactnumber")
                        };
                    }
                }
            }
            catch (Exception e)
            {
                // TODO
            }

            return owner;
        }

        public List<PetOwner> GetBySearchQuery(string query)
        {
            Connection = null;
            Command = null;
            Reader = null;

            List<PetOwner> list = new();

            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = SearchByFullName;
                    Command.Parameters.AddWithValue("@name", "%" + query + "%");
                    Reader = Command.ExecuteReader();

                    while (Reader.Read())
                    {
                        list.Add(new PetOwner()
                        {
                            Id = Reader.GetInt32("id"),
                            FullName = Reader.GetString("fullname"),
                            Email = Reader.GetString("email"),
                            Contact = Reader.GetString("contactnumber")
                        });
                    }
                }
            }
            catch (Exception e)
            {
                // TODO
            }
            return list;
        }

        public bool Update(PetOwner entity)
        {
            if (entity == null)
                return false;

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
                    Command.Parameters.AddWithValue("@name", entity.FullName);
                    Command.Parameters.AddWithValue("@email", entity.Email);
                    Command.Parameters.AddWithValue("@contact", entity.Contact);
                    Command.Parameters.AddWithValue("@id", entity.Id);

                    return Command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception e)
            {
                // TODO
                return false;
            }
        }
    }
}
