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
    public class MySqlAppointmentDao : IAppointmentDao
    {
        MySqlConnection? Connection;
        MySqlCommand? Command;
        MySqlDataReader? Reader;

        private IPetDao PetDao = DaoFactory.Instance(DaoType.MySql).Pets;
        private IVeterinarianDao VetDao = DaoFactory.Instance(DaoType.MySql).Veterinarians;

        private static readonly string SelectAll = "SELECT * FROM appointment ORDER BY datetime";
        private static readonly string SelectById = "SELECT * FROM WHERE id=@id ORDER BY datetime";
        private static readonly string SelectAllWithScheduling = "SELECT * FROM appointment WHERE scheduled=@scheduled ORDER BY datetime";
        private static readonly string SelectAllByVet = SelectAll + " WHERE vet=@vet";
        private static readonly string Insert = "INSERT INTO appointment(datetime, reason, pet, vet) VALUES(@date, @reason, @pet, @vet)";
        private static readonly string DeleteId = "DELETE FROM appointment WHERE id=@id";
        private static readonly string UpdateById = "UPDATE appointment set datetime=@date, reason=@reason, vet=@vet WHERE id=@id";

        public int Create(Appointment entity)
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
                    Command.Parameters.AddWithValue("@date", entity.DateTime);
                    Command.Parameters.AddWithValue("@reason", entity.Reason);
                    Command.Parameters.AddWithValue("@pet", entity.Pet.Id);
                    Command.Parameters.AddWithValue("@vet", entity.Vet.User.Id);
                    Command.ExecuteNonQuery();
                    return (int)Command.LastInsertedId;
                }
            }
            catch (Exception)
            {
                // TODO
                return -1;
            }
        }

        public bool Update(Appointment entity)
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
                    Command.Parameters.AddWithValue("@date", entity.DateTime);
                    Command.Parameters.AddWithValue("@reason", entity.Reason);
                    Command.Parameters.AddWithValue("@vet", entity.Vet.User.Id);;

                    return Command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception)
            {
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
                return false;
            }
        }

        public List<Appointment> GetAll()
        {
            Connection = null;
            Command = null;
            Reader = null;

            List<Appointment> list = new();
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
                        list.Add(new Appointment()
                        {
                            Id = Reader.GetInt32("id"),
                            DateTime = Reader.GetDateTime("datetime"),
                            Reason = Reader.GetString("reason"),
                            IsScheduled = Reader.GetBoolean("scheduled"),
                            Pet = PetDao.GetById(Reader.GetInt32("pet")),
                            Vet = VetDao.GetById(Reader.GetInt32("vet"))
                        });
                    }

                    Reader.Close();
                }
            }
            catch (Exception) { }

            return list;
        }

        public Appointment GetById(int id)
        {
            Connection = null;
            Command = null;
            Reader = null;

            Appointment app = null;
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
                        app = new Appointment()
                        {
                            Id = Reader.GetInt32("id"),
                            DateTime = Reader.GetDateTime("datetime"),
                            Reason = Reader.GetString("reason"),
                            IsScheduled = Reader.GetBoolean("scheduled"),
                            Pet = PetDao.GetById(Reader.GetInt32("pet")),
                            Vet = VetDao.GetById(Reader.GetInt32("vet"))
                        };
                    }

                    Reader.Close();
                }
            }
            catch (Exception) { }

            return app;
        }

        public List<Appointment> GetByScheduled(bool scheduled)
        {
            Connection = null;
            Command = null;
            Reader = null;

            List<Appointment> list = new();
            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = SelectAllWithScheduling;
                    Command.Parameters.AddWithValue("@scheduled", scheduled);
                    Reader = Command.ExecuteReader();

                    while (Reader.Read())
                    {
                        list.Add(new Appointment()
                        {
                            Id = Reader.GetInt32("id"),
                            DateTime = Reader.GetDateTime("datetime"),
                            Reason = Reader.GetString("reason"),
                            IsScheduled = Reader.GetBoolean("scheduled"),
                            Pet = PetDao.GetById(Reader.GetInt32("pet")),
                            Vet = VetDao.GetById(Reader.GetInt32("vet"))
                        });
                    }

                    Reader.Close();
                }
            }
            catch (Exception) { }

            return list;
        }

        public List<Appointment> GetByVetId(int id)
        {
            Connection = null;
            Command = null;
            Reader = null;

            List<Appointment> list = new();
            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = SelectAllByVet;
                    Command.Parameters.AddWithValue("@vet", id);
                    Reader = Command.ExecuteReader();

                    while (Reader.Read())
                    {
                        list.Add(new Appointment()
                        {
                            Id = Reader.GetInt32("id"),
                            DateTime = Reader.GetDateTime("datetime"),
                            Reason = Reader.GetString("reason"),
                            IsScheduled = Reader.GetBoolean("scheduled"),
                            Pet = PetDao.GetById(Reader.GetInt32("pet")),
                            Vet = VetDao.GetById(Reader.GetInt32("vet"))
                        });
                    }

                    Reader.Close();
                }
            }
            catch (Exception) { }

            return list;
        }

        public List<Appointment> GetByVetNameSurname(string name)
        {
            return GetAll().Where(a =>
            {
                string full = a.Vet.User.Name + " " + a.Vet.User.Surname;
                return full.ToLower().Contains(name.ToLower());
            }).ToList();
        }

        public List<Appointment> GetByOwnerFullName(string query) => GetAll().Where(a => a.Pet.Owner.FullName.ToLower().Contains(query.ToLower())).ToList();

        public bool Schedule(Appointment appointment, string address)
        {
            Examination examination = new Examination()
            {
                Id = 0,
                DateTime = appointment.DateTime,
                Address = address,
                IsCompleted = false,
                Appointment = appointment,
                Vet = appointment.Vet,
                Pet = appointment.Pet
            };

            Connection = null;
            Command = null;
            Reader = null;

            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = "INSERT INTO examination(datetime, address, pet, vet, appointment) VALUES(@date, @address, @pet, @vet, @appointment)";
                    Command.Parameters.AddWithValue("@date", examination.DateTime);
                    Command.Parameters.AddWithValue("@address", examination.Address);
                    Command.Parameters.AddWithValue("@pet", examination.Pet.Id);
                    Command.Parameters.AddWithValue("@vet", examination.Vet.User.Id);
                    Command.Parameters.AddWithValue("@appointment", appointment.Id);
                    Command.ExecuteNonQuery();

                    if (((int)Command.LastInsertedId) > 0)
                    {
                        Command = Connection.CreateCommand();
                        Command.CommandText = "UPDATE appointment set scheduled=true, datetime=@date WHERE id=" + appointment.Id;
                        Command.Parameters.Clear();
                        Command.Parameters.AddWithValue("@date", appointment.DateTime);
                        return Command.ExecuteNonQuery() > 0;
                    }
                    else return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<Appointment> GetBySpecs(string owner, string vet, bool scheduled)
        {
            List<Appointment> list = GetByScheduled(scheduled);
            return list.Where(a => a.Pet.Owner.FullName.ToLower().Contains(owner.ToLower()) && a.Vet.ToString().ToLower().Contains(vet.ToLower())).ToList();
        }
    }
}
