using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using VetClinic.Models.Entities;
using VetClinic.Utils;

namespace VetClinic.Dao.MySqlDao
{
    public class MySqlAdministratorDao : IAdministratorDao
    {
        MySqlConnection? Connection;
        MySqlCommand? Command;
        MySqlDataReader? Reader;

        private static readonly string SelectAll = "SELECT * FROM administrator NATURAL INNER JOIN user";
        private static readonly string SelectByUsernameAndPassword = SelectAll + " WHERE username=@usnm AND password=@passwd";
        private static readonly string SelectById = SelectAll + "WHERE id=@identif";
        private static readonly string UpdateThemeAndLang = "UPDATE user SET theme=@th, language=@lang WHERE id=@identif";

        public Administrator GetByUsernameAndPassword(string username, string password)
        {
            Connection = null;
            Command = null;
            Reader = null;

            Administrator administrator = null;
            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = SelectByUsernameAndPassword;
                    Command.Parameters.AddWithValue("@usnm", username);
                    Command.Parameters.AddWithValue("@passwd", password);
                    Reader = Command.ExecuteReader();

                    if(Reader.Read())
                    {
                        administrator = new Administrator()
                        {
                            User = new User()
                            {
                                Id = Reader.GetInt32("id"),
                                Name = Reader.GetString("name"),
                                Surname = Reader.GetString("surname"),
                                Username = Reader.GetString("username"),
                                Password = Reader.GetString("password"),
                                Email = Reader.GetString("email"),
                                Contact = Reader.GetString("contact"),
                                Theme = Reader.GetString("theme"),
                                Language = Reader.GetString("language")
                            }
                        };
                    }
                }
            } catch(Exception e)
            {
                // TODO
            }

            return administrator;
        }

        public Administrator GetById(int id)
        {
            Connection = null;
            Command = null;
            Reader = null;

            Administrator administrator = null;
            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = SelectById;
                    Command.Parameters.AddWithValue("@identif", id);
                    Reader = Command.ExecuteReader();

                    if (Reader.Read())
                    {
                        administrator = new Administrator()
                        {
                            User = new User()
                            {
                                Id = Reader.GetInt32("id"),
                                Name = Reader.GetString("name"),
                                Surname = Reader.GetString("surname"),
                                Username = Reader.GetString("username"),
                                Password = Reader.GetString("password"),
                                Email = Reader.GetString("email"),
                                Contact = Reader.GetString("contact"),
                                Theme = Reader.GetString("theme"),
                                Language = Reader.GetString("language")
                            }
                        };
                    }
                }
            }
            catch (Exception e)
            {
                // TODO
            }

            return administrator;
        }

        public List<Administrator> GetAll()
        {
            Connection= null;
            Command= null;
            Reader= null;

            List<Administrator> list = new List<Administrator>();
            try
            {
                using(Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = SelectAll;
                    Reader = Command.ExecuteReader();

                    while(Reader.Read())
                    {
                        list.Add(new Administrator()
                        {
                            User = new User()
                            {
                                Id = Reader.GetInt32("id"),
                                Name = Reader.GetString("name"),
                                Surname = Reader.GetString("surname"),
                                Username = Reader.GetString("username"),
                                Password = Reader.GetString("password"),
                                Email = Reader.GetString("email"),
                                Contact = Reader.GetString("contact"),
                                Theme = Reader.GetString("theme"),
                                Language = Reader.GetString("language")
                            }
                        });
                    }
                }
            } catch(Exception e)
            {
                // TODO
            }

            return list;
        }

        public bool Update(Administrator entity)
        {
            if(entity == null || entity.User == null)
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
                    Command.CommandText = UpdateThemeAndLang;
                    Command.Parameters.AddWithValue("@identif", entity.User.Id);
                    Command.Parameters.AddWithValue("@th", entity.User.Theme);
                    Command.Parameters.AddWithValue("@lang", entity.User.Language);

                    return Command.ExecuteNonQuery() > 0;
                }
            } catch(Exception e )
            {
                // TODO
                return false;
            }
        }

        public int Create(Administrator entity)
        {
            throw new NotImplementedException();
        }

        public bool DeleteById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
