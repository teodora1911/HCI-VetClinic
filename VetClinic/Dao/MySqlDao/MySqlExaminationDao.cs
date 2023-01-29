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
    public class MySqlExaminationDao : IExaminationDao
    {
        MySqlConnection? Connection;
        MySqlCommand? Command;
        MySqlDataReader? Reader;

        private IPetDao PetDao = DaoFactory.Instance(DaoType.MySql).Pets;
        private IVeterinarianDao VetDao = DaoFactory.Instance(DaoType.MySql).Veterinarians;
        private IAppointmentDao AppDao = DaoFactory.Instance(DaoType.MySql).Appointments;

        private static readonly string SelectAll = "SELECT * FROM examination";
        private static readonly string SelectById = SelectAll + " WHERE id=@id";
        private static readonly string InsertWithAppointment = "INSERT INTO examination(datetime, address, pet, vet, appointment) VALUES(@date, @address, @pet, @vet, @appointment)";
        private static readonly string InsertWithoutAppointment = "INSERT INTO examination(datetime, address, pet, vet) VALUES (@date, @address, @pet, @vet)";
        private static readonly string CompleteAppointment = "UPDATE examination set completed=@completed WHERE id=@id";

        public int Create(Examination entity)
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
                    Command.CommandText = entity.Appointment == null ? InsertWithoutAppointment : InsertWithAppointment;
                    Command.Parameters.AddWithValue("@date", entity.DateTime);
                    Command.Parameters.AddWithValue("@address", entity.Address);
                    Command.Parameters.AddWithValue("@pet", entity.Pet.Id);
                    Command.Parameters.AddWithValue("@vet", entity.Vet.User.Id);
                    if (entity.Appointment != null)
                        Command.Parameters.AddWithValue("@appointment", entity.Appointment.Id);
                    Command.ExecuteNonQuery();
                    return (int)Command.LastInsertedId;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public bool Update(Examination entity)
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
                    Command.CommandText = CompleteAppointment;
                    Command.Parameters.AddWithValue("@id", entity.Id);
                    Command.Parameters.AddWithValue("@completed", entity.IsCompleted);
                    return Command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteById(int id) => throw new NotImplementedException();

        public List<Examination> GetAll()
        {
            Connection = null;
            Command = null;
            Reader = null;

            List<Examination> list = new();
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
                        list.Add(new Examination()
                        {
                            Id = Reader.GetInt32("id"),
                            DateTime = Reader.GetDateTime("datetime"),
                            Address = Reader.GetString("address"),
                            IsCompleted = Reader.GetBoolean("completed"),
                            Pet = PetDao.GetById(Reader.GetInt32("pet")),
                            Vet = VetDao.GetById(Reader.GetInt32("vet")),
                            Appointment = new()
                        });
                    }

                    Reader.Close();
                }
            }
            catch (Exception) { }

            return list;
        }

        public Examination GetById(int id)
        {
            Connection = null;
            Command = null;
            Reader = null;

            Examination exam = null;
            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = SelectById;
                    Command.Parameters.AddWithValue("@id", id);
                    Reader = Command.ExecuteReader();

                    while (Reader.Read())
                    {
                        exam = new Examination()
                        {
                            Id = Reader.GetInt32("id"),
                            DateTime = Reader.GetDateTime("datetime"),
                            Address = Reader.GetString("address"),
                            IsCompleted = Reader.GetBoolean("completed"),
                            Pet = PetDao.GetById(Reader.GetInt32("pet")),
                            Vet = VetDao.GetById(Reader.GetInt32("vet")),
                            Appointment = new()
                        };
                    }

                    Reader.Close();
                }
            }
            catch (Exception) { }

            return exam;
        }

        public Examination GetByAppointmentId(int appointmentId)
        {
            Connection = null;
            Command = null;
            Reader = null;
            
            Examination exam = null;
            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = "SELECT * FROM examination where appointment=@app";
                    Command.Parameters.AddWithValue("@app", appointmentId);
                    Reader = Command.ExecuteReader();

                    if (Reader.Read())
                    {
                        exam = new Examination()
                        {
                            Id = Reader.GetInt32("id"),
                            DateTime = Reader.GetDateTime("datetime"),
                            Address = Reader.GetString("address"),
                            IsCompleted = Reader.GetBoolean("completed"),
                            Pet = PetDao.GetById(Reader.GetInt32("pet")),
                            Vet = VetDao.GetById(Reader.GetInt32("vet")),
                            Appointment = new()
                        };
                    }
                }
            }
            catch (Exception) { }

            return exam;
        }
    }
}
