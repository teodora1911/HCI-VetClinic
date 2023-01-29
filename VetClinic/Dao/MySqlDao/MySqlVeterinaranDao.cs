using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using VetClinic.Models.Entities;
using VetClinic.Utils;

namespace VetClinic.Dao.MySqlDao
{
    public class MySqlVeterinaranDao : IVeterinarianDao
    {
        MySqlConnection? Connection;
        MySqlCommand? Command;
        MySqlDataReader? Reader;

        private static readonly string SelectAll = "SELECT * FROM vet NATURAL INNER JOIN user";
        private static readonly string SelectById = SelectAll + " WHERE id=@identif";
        private static readonly string SelectByUsernameAndPassword = SelectAll + " WHERE username=@usnm AND password=@passwd";
        private static readonly string IsUsernameAvailable = "SELECT COUNT(*) as count FROM user WHERE username=@username";
        private static readonly string InsertUser = "INSERT INTO user(name, surname, username, password, email, contact) VALUES (@name, @surname, @username, @password, @email, @contact)";
        private static readonly string InsertVet = "INSERT INTO vet(id, title) VALUES (@id, @title)";
//        private static readonly string UpdateThemeAndLang = "UPDATE user SET theme=@th, language=@lang WHERE id=@identif";
        private static readonly string UpdateUser = "UPDATE user SET name=@name, surname=@surname, password=@passwd, email=@email, contact=@contact, theme=@th, language=@lang WHERE id=@id";
        private static readonly string UpdateVet = "UPDATE vet SET title=@title WHERE id=@id";
        private static readonly string DeleteId = "DELETE FROM vet, user USING vet INNER JOIN user ON vet.id=user.id  WHERE vet.id=@id";
        private static readonly string SearchNameSurname = SelectAll + " WHERE name LIKE @name AND surname LIKE @surname";
        private static readonly string TitleList = "SELECT DISTINCT title FROM vet";

        public int Create(Veterinarian entity)
        {
            Connection = null;
            Command = null;
            Reader = null;

            try
            {
                using(Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();

                    // Check if username is valid
                    Command = Connection.CreateCommand();
                    Command.CommandText = IsUsernameAvailable;
                    Command.Parameters.AddWithValue("@username", entity.User.Username);
                    Reader = Command.ExecuteReader();
                    if (Reader.Read())
                    {
                        int count = Reader.GetInt32("count");
                        Reader.Close();

                        if (count > 0)
                            return 0;

                        // Insert values into user table
                        Command = Connection.CreateCommand();
                        Command.CommandText = InsertUser;
                        Command.Parameters.Clear();
                        Command.Parameters.AddWithValue("@name", entity.User.Name);
                        Command.Parameters.AddWithValue("@surname", entity.User.Surname);
                        Command.Parameters.AddWithValue("@username", entity.User.Username);
                        Command.Parameters.AddWithValue("@password", entity.User.Password);
                        Command.Parameters.AddWithValue("@email", entity.User.Email);
                        Command.Parameters.AddWithValue("@contact", entity.User.Contact);
                        Command.ExecuteNonQuery();
                        int id = (int)Command.LastInsertedId;

                        // Insert values into vet table
                        Command = Connection.CreateCommand();
                        Command.CommandText = InsertVet;
                        Command.Parameters.Clear();
                        Command.Parameters.AddWithValue("@id", id);
                        Command.Parameters.AddWithValue("@title", entity.Title);

                        return Command.ExecuteNonQuery();
                    }
                    else throw new Exception();
                }
            } catch(Exception e)
            {
                // TODO
                return -1;
            }
        }

        public List<Veterinarian> GetAll()
        {
            Connection = null;
            Command = null;
            Reader = null;

            List<Veterinarian> list = new();
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
                        list.Add(new Veterinarian()
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
                            },
                            Title = Reader.GetString("title")
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

        public Veterinarian GetById(int id)
        {
            Connection = null;
            Command = null;
            Reader = null;

            Veterinarian veterinarian = null;
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
                        veterinarian = new Veterinarian()
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
                            },
                            Title = Reader.GetString("title")
                        };
                    }
                }
            }
            catch (Exception e)
            {
                // TODO
            }

            return veterinarian;
        }

        public Veterinarian GetByUsernameAndPassword(string username, string password)
        {
            Connection = null;
            Command = null;
            Reader = null;

            Veterinarian veterinarian = null;
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

                    if (Reader.Read())
                    {
                        veterinarian = new Veterinarian()
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
                            },
                            Title= Reader.GetString("title")
                        };
                    }
                }
            }
            catch (Exception e)
            {
                // TODO
            }

            return veterinarian;
        }

        public bool Update(Veterinarian entity)
        {
            if (entity == null || entity.User == null)
                return false;
            Connection = null;
            Command = null;
            Reader = null;

            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();

                    // Update user Table
                    Command = Connection.CreateCommand();
                    Command.CommandText = UpdateUser;
                    Command.Parameters.AddWithValue("@id", entity.User.Id);
                    Command.Parameters.AddWithValue("@th", entity.User.Theme);
                    Command.Parameters.AddWithValue("@lang", entity.User.Language);
                    Command.Parameters.AddWithValue("@name", entity.User.Name);
                    Command.Parameters.AddWithValue("@surname", entity.User.Surname);
                    Command.Parameters.AddWithValue("@passwd", entity.User.Password);
                    Command.Parameters.AddWithValue("@email", entity.User.Email);
                    Command.Parameters.AddWithValue("@contact", entity.User.Contact);

                    if (Command.ExecuteNonQuery() > 0)
                    {
                        // Update vet Table
                        Command = Connection.CreateCommand();
                        Command.CommandText = UpdateVet;
                        Command.Parameters.Clear();
                        Command.Parameters.AddWithValue("@id", entity.User.Id);
                        Command.Parameters.AddWithValue("@title", entity.Title);

                        return Command.ExecuteNonQuery() > 0;
                    }
                    else return false;
                }
            }
            catch (Exception e)
            {
                // TODO
                return false;
            }
        }

        public bool DeleteById(int id)
        {
            Connection = null;
            Command = null;
            Reader = null;

            try
            {
                using(Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = DeleteId;
                    Command.Parameters.AddWithValue("@id", id);

                    return Command.ExecuteNonQuery() == 2;
                }
            } catch(Exception e)
            {
                // TODO
                return false;
            }
        }

        public List<string> GetTitles()
        {
            Connection = null;
            Command = null;
            Reader = null;

            List<string> titles = new();

            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = TitleList;
                    Reader = Command.ExecuteReader();

                    while (Reader.Read())
                        titles.Add(Reader.GetString("title"));
                }
            }
            catch (Exception e)
            {
                // TODO
            }

            return titles;
        }

        public List<Veterinarian> GetBySearchQuery(string query)
        {
            Connection = null;
            Command = null;
            Reader = null;

            List<Veterinarian> list = new();

            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    string[] args = query.Split(" ");
                    string name = args[0];
                    string surname = "";
                    if (args.Length > 1)
                        surname = args[1];
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = SearchNameSurname;
                    Command.Parameters.AddWithValue("@name", "%" + name + "%");
                    Command.Parameters.AddWithValue("@surname", "%" + surname + "%");
                    Reader = Command.ExecuteReader();

                    while (Reader.Read())
                    {
                        list.Add(new Veterinarian()
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
                            },
                            Title = Reader.GetString("title")
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

//        public List<Veterinarian> GetByTitle(string title) => Search("", title);

//        public List<Veterinarian> GetByTitleAndQuery(string title, string query) => Search(query, title);
    }
}
