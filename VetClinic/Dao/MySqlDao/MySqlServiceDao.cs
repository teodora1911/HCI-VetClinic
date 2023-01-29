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
    public class MySqlServiceDao : IServiceDao
    {

        MySqlConnection? Connection;
        MySqlCommand? Command;
        MySqlDataReader? Reader;

        private static readonly string SelectAll = "SELECT * FROM service";
        private static readonly string SelectById = SelectAll + " WHERE id=@id";
        private static readonly string SearchByName = SelectAll + " WHERE name LIKE @name";
        private static readonly string UpdateService = "UPDATE service set name=@name, cost=@cost, description=@desc WHERE id=@id";
        private static readonly string Insert = "INSERT INTO service(name, cost, description) VALUES(@name, @cost, @desc)";
        private static readonly string Delete = "DELETE FROM service WHERE id=@id";

        public int Create(Service entity)
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
                    Command.Parameters.AddWithValue("@name", entity.Name);
                    Command.Parameters.AddWithValue("@desc", entity.Description);
                    Command.Parameters.AddWithValue("@cost", entity.Cost);
                    Command.ExecuteNonQuery();
                    return (int)Command.LastInsertedId;
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

        public List<Service> GetAll()
        {
            Connection = null;
            Command = null;
            Reader = null;

            List<Service> list = new();
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
                        list.Add(new Service()
                        {
                            Id = Reader.GetInt32("id"),
                            Name = Reader.GetString("name"),
                            Description = Reader.GetString("description"),
                            Cost = Reader.GetDecimal("cost")
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

        public Service GetById(int id)
        {
            Connection = null;
            Command = null;
            Reader = null;

            Service service = null;
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
                        service = new Service()
                        {
                            Id = Reader.GetInt32("id"),
                            Name = Reader.GetString("name"),
                            Description = Reader.GetString("description"),
                            Cost = Reader.GetDecimal("cost")
                        };
                    }
                }
            }
            catch (Exception e)
            {
                // TODO
            }

            return service;
        }

        public List<Service> GetBySearchQuery(string query)
        {
            Connection = null;
            Command = null;
            Reader = null;

            List<Service> list = new();

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
                        list.Add(new Service()
                        {
                            Id = Reader.GetInt32("id"),
                            Name = Reader.GetString("name"),
                            Description = Reader.GetString("description"),
                            Cost = Reader.GetDecimal("cost")
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

        public bool Update(Service entity)
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
                    Command.CommandText = UpdateService;
                    Command.Parameters.AddWithValue("@name", entity.Name);
                    Command.Parameters.AddWithValue("@desc", entity.Description);
                    Command.Parameters.AddWithValue("@cost", entity.Cost);
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
