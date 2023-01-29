using MaterialDesignColors;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.Models.Entities;
using VetClinic.Utils;

namespace VetClinic.Dao.MySqlDao
{
    public class MySqlMedicineDao : IMedicineDao
    {
        MySqlConnection? Connection;
        MySqlCommand? Command;
        MySqlDataReader? Reader;

        private static readonly string SelectAll = "SELECT * FROM medicine";
        private static readonly string SelectById = SelectAll + " WHERE id=@id";
        private static readonly string SearchByName = SelectAll + " WHERE name LIKE @name";
        private static readonly string SearchByType = SelectAll + " WHERE type LIKE @type";
        private static readonly string UpdateId = "UPDATE medicine set name=@name, description=@desc, type=@type WHERE id=@id";
        private static readonly string Insert = "INSERT INTO medicine(name, description, type) VALUES(@name, @desc, @type)";
        private static readonly string DeleteId = "DELETE FROM medicine WHERE id=@id";

        private static readonly string SelectTypes = "SELECT DISTINCT type FROM medicine";

        public int Create(Medicine entity)
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
                    Command.Parameters.AddWithValue("@type", entity.Type);
                    Command.ExecuteNonQuery();
                    return (int)Command.LastInsertedId;
                }
            } catch(Exception)
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
                    Command.CommandText = DeleteId;
                    Command.Parameters.AddWithValue("@id", id);

                    return Command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception)
            {
                // TODO
                return false;
            }
        }

        public List<Medicine> GetAll()
        {
            Connection = null;
            Command = null;
            Reader = null;

            List<Medicine> list = new();
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
                        list.Add(new Medicine()
                        {
                            Id = Reader.GetInt32("id"),
                            Name = Reader.GetString("name"),
                            Description = Reader.GetString("description"),
                            Type = Reader.GetString("type")
                        });
                    }
                }
            }
            catch (Exception)
            {
                // TODO
            }

            return list;
        }

        public Medicine GetById(int id)
        {
            Connection = null;
            Command = null;
            Reader = null;

            Medicine medicine = null;
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
                        medicine = new Medicine()
                        {
                            Id = Reader.GetInt32("id"),
                            Name = Reader.GetString("name"),
                            Description = Reader.GetString("description"),
                            Type = Reader.GetString("type")
                        };
                    }
                }
            }
            catch (Exception)
            {
                // TODO
            }

            return medicine;
        }

        public List<Medicine> GetBySearchQuery(string query)
        {
            Connection = null;
            Command = null;
            Reader = null;

            List<Medicine> list = new();

            try
            {
                using(Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();   
                    Command.CommandText = SearchByName;
                    Command.Parameters.AddWithValue("@name", "%" + query + "%");
                    Reader = Command.ExecuteReader();

                    while(Reader.Read())
                    {
                        list.Add(new Medicine()
                        {
                            Id = Reader.GetInt32("id"),
                            Name = Reader.GetString("name"),
                            Description = Reader.GetString("description"),
                            Type = Reader.GetString("type")
                        });
                    }
                }
            } catch(Exception)
            {
                // TODO
            }
            return list;
        }

        public List<Medicine> GetByType(string type)
        {
            Connection = null;
            Command = null;
            Reader = null;

            List<Medicine> list = new();

            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = SearchByType;
                    Command.Parameters.AddWithValue("@type", "%" + type + "%");
                    Reader = Command.ExecuteReader();

                    while (Reader.Read())
                    {
                        list.Add(new Medicine()
                        {
                            Id = Reader.GetInt32("id"),
                            Name = Reader.GetString("name"),
                            Description = Reader.GetString("description"),
                            Type = Reader.GetString("type")
                        });
                    }
                }
            }
            catch (Exception)
            {
                // TODO
            }
            return list;
        }

        public List<string> GetTypes()
        {
            Connection = null;
            Command = null;
            Reader = null;

            List<string> types = new();

            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = SelectTypes;
                    Reader = Command.ExecuteReader();

                    while (Reader.Read())
                        types.Add(Reader.GetString("type"));
                }
            }
            catch (Exception)
            {
                // TODO
            }

            return types;
        }

        public List<Medicine> GetByNameAndType(string name, string type)
        {
            List<Medicine> NameList = GetBySearchQuery(name);
            List<Medicine> TypeList = GetByType(type);

            return NameList.Intersect(TypeList).ToList();
        }

        public bool Update(Medicine entity)
        {
            if(entity == null)
                return false;

            Connection = null;
            Command = null;
            Reader = null;

            try
            {
                using(Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = UpdateId;
                    Command.Parameters.AddWithValue("@name", entity.Name);
                    Command.Parameters.AddWithValue("@desc", entity.Description);
                    Command.Parameters.AddWithValue("@type", entity.Type);
                    Command.Parameters.AddWithValue("@id", entity.Id);

                    return Command.ExecuteNonQuery() > 0;
                }
            } catch(Exception)
            {
                // TODO
                return false;
            }
        }
    }
}
