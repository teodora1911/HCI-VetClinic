using MaterialDesignThemes.Wpf;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
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
        private IServiceDao ServiceDao = DaoFactory.Instance(DaoType.MySql).Services;
        private IMedicineDao MedicineDao = DaoFactory.Instance(DaoType.MySql).Medicine;
        private IPetOwnerDao OwnerDao = DaoFactory.Instance(DaoType.MySql).PetOwners;

        private static readonly string SelectAll = "SELECT * FROM examination WHERE deleted=false ORDER BY datetime";
        private static readonly string SelectById = "SELECT * FROM examination WHERE id=@id AND deleted=false ORDER BY datetime";
        private static readonly string SelectByVeterinarianId = "SELECT * FROM examination WHERE vet=@vet AND deleted=false ORDER BY datetime";
        private static readonly string InsertWithAppointment = "INSERT INTO examination(datetime, address, pet, vet, appointment) VALUES(@date, @address, @pet, @vet, @appointment)";
        private static readonly string InsertWithoutAppointment = "INSERT INTO examination(datetime, address, pet, vet) VALUES (@date, @address, @pet, @vet)";
        private static readonly string CompleteExamination = "UPDATE examination SET completed=true WHERE id=@id";
        private static readonly string DeleteExamination = "UPDATE examination SET deleted=true WHERE id=@id";

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
                    Command.CommandText = CompleteExamination;
                    Command.Parameters.AddWithValue("@id", entity.Id);
//                    Command.Parameters.AddWithValue("@completed", entity.IsCompleted);
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
                    Command.CommandText = DeleteExamination;
                    Command.Parameters.AddWithValue("@id", id);
                    return Command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception) { return false; }
        }

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

        public List<Examination> GetAllFromVeterinarian(int veterinarianId)
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
                    Command.CommandText = SelectByVeterinarianId;
                    Command.Parameters.AddWithValue("@vet", veterinarianId);
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

        public List<Examination> GetAllFromVetAndSearchPet(int vet, string search, bool completed) 
            => GetAllFromVeterinarian(vet).Where(e => e.Pet.Name.ToLower().Contains(search.ToLower()) && (e.IsCompleted == completed)).ToList();

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

        private static readonly string SelectAllServicesExaminationByExamination = "SELECT * FROM examinationservice WHERE examination=@id";
        private static readonly string SelectAllPrescriptionByExamination = "SELECT * FROM prescription WHERE examination=@id";

        private static readonly string InsertExaminationService = "INSERT INTO examinationservice(examination, service, quantity, cost) VALUES (@exam, @service, @quantity, @cost)";
        private static readonly string InsertPrescription = "INSERT INTO prescription(examination, medicine, name, dose, frequency, start, duration, instructions) VALUES (@exam, @medicine, @name, @dose, @frequency, @start, @duration, @instructions)";

        private static readonly string UpdateExaminationServiceQuery = "UPDATE examinationservice SET quantity=@quantity, cost=@cost WHERE examination=@exam AND service=@service";
        private static readonly string UpdatePrescriptionQuery = "UPDATE prescription SET name=@name, dose=@dose, fequency=@frequency, start=@start, duration=@duration, instructions=@instructions WHERE examination=@exam AND medicine=@medicine";

        private static readonly string DeleteExaminationServiceQuery = "DELETE FROM examinationservice WHERE examination=@exam AND service=@service";
        private static readonly string DeletePrescriptionQuery = "DELETE FROM prescription WHERE examination=@exam AND medicine=@medicine";

        public List<ExaminationService> GetAllSevicesFromExamination(int id)
        {
            Connection = null;
            Command = null;
            Reader = null;

            List<ExaminationService> list = new();
            try
            {
                using(Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = SelectAllServicesExaminationByExamination;
                    Command.Parameters.AddWithValue("@id", id);
                    Reader = Command.ExecuteReader();

                    while (Reader.Read())
                    {
                        list.Add(new ExaminationService()
                        {
                            Examination = id,
                            Service = ServiceDao.GetById(Reader.GetInt32("service")),
                            Quantity = Reader.GetInt32("quantity"),
                            Cost = Reader.GetDecimal("cost")
                        });
                    }

                    Reader.Close();
                }
            }
            catch(Exception) { }
            return list;
        }

        public List<Prescription> GetAllPrescriptionsFromExamination(int id)
        {
            Connection = null;
            Command = null;
            Reader = null;

            List<Prescription> list = new();
            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = SelectAllPrescriptionByExamination;
                    Command.Parameters.AddWithValue("@id", id);
                    Reader = Command.ExecuteReader();

                    while (Reader.Read())
                    {
                        list.Add(new Prescription()
                        {
                            Examination = id,
                            Medicine = MedicineDao.GetById(Reader.GetInt32("medicine")),
                            Name = Reader.GetString("name"),
                            Dose = Reader.GetInt32("dose"),
                            Frequency = Reader.GetString("frequency"),
                            Start = Reader.GetDateTime("start"),
                            Duration = Reader.GetString("duration"),
                            Instructions = Reader.GetString("instructions")
                        });
                    }

                    Reader.Close();
                }
            }
            catch (Exception) { }
            return list;
        }

        public bool AddService(ExaminationService entity)
        {
            Connection = null;
            Command = null;
            Reader = null;

            entity.Cost = entity.Quantity * entity.Service.Cost;

            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = InsertExaminationService;
                    // (@exam, @service, @quantity, @cost)
                    Command.Parameters.AddWithValue("@exam", entity.Examination);
                    Command.Parameters.AddWithValue("@service", entity.Service.Id);
                    Command.Parameters.AddWithValue("@quantity", entity.Quantity);
                    Command.Parameters.AddWithValue("@cost", entity.Cost);

                    return Command.ExecuteNonQuery() > 0;
                }
            }
            catch(Exception) { return false; }
        }

        public bool AddPrescription(Prescription prescription)
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
                    Command.CommandText = InsertPrescription;
                    // (@exam, @medicine, @name, @dose, @frequency, @start, @duration, @instructions)
                    Command.Parameters.AddWithValue("@exam", prescription.Examination);
                    Command.Parameters.AddWithValue("@medicine", prescription.Medicine.Id);
                    Command.Parameters.AddWithValue("@name", prescription.Name);
                    Command.Parameters.AddWithValue("@dose", prescription.Dose);
                    Command.Parameters.AddWithValue("@frequency", prescription.Frequency);
                    Command.Parameters.AddWithValue("@start", prescription.Start);
                    Command.Parameters.AddWithValue("@duration", prescription.Duration);
                    Command.Parameters.AddWithValue("@instructions", prescription.Instructions);

                    return Command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception) { return false; }
        }

        public bool UpdateService(ExaminationService entity)
        {
            Connection = null;
            Command = null;
            Reader = null;

            entity.Cost = entity.Quantity * entity.Service.Cost;

            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = UpdateExaminationServiceQuery;
                    // quantity = @quantity, cost = @cost WHERE examination = @exam AND service = @service
                    Command.Parameters.AddWithValue("@quantity", entity.Quantity);
                    Command.Parameters.AddWithValue("@cost", entity.Cost);
                    Command.Parameters.AddWithValue("@exam", entity.Examination);
                    Command.Parameters.AddWithValue("@service", entity.Service.Id);

                    return Command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdatePrescription(Prescription entity)
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
                    Command.CommandText = UpdatePrescriptionQuery;
                    // name=@name, dose=@dose, fequency=@frequency, start=@start, duration=@duration, instructions=@instructions WHERE examination=@exam AND medicine=@medicine
                    Command.Parameters.AddWithValue("@name", entity.Name);
                    Command.Parameters.AddWithValue("@dose", entity.Dose);
                    Command.Parameters.AddWithValue("@frequency", entity.Frequency);
                    Command.Parameters.AddWithValue("@start", entity.Start);
                    Command.Parameters.AddWithValue("@duration", entity.Duration);
                    Command.Parameters.AddWithValue("@instructions", entity.Instructions);
                    Command.Parameters.AddWithValue("@exam", entity.Examination);
                    Command.Parameters.AddWithValue("@medicine", entity.Medicine.Id);

                    return Command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteExaminationService(ExaminationService examinationService)
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
                    Command.CommandText = DeleteExaminationServiceQuery;
                    Command.Parameters.AddWithValue("@exam", examinationService.Examination);
                    Command.Parameters.AddWithValue("@service", examinationService.Service.Id);

                    return Command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception) { return false; }
        }

        public bool DeletePrescription(Prescription prescription)
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
                    Command.CommandText = DeletePrescriptionQuery;
                    Command.Parameters.AddWithValue("@exam", prescription.Examination);
                    Command.Parameters.AddWithValue("@medicine", prescription.Medicine.Id);

                    return Command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception) { return false; }
        }

        private static readonly string InsertBillQuery = "INSERT INTO bill(totalprice, timestamp, payment, examination, user) VALUES(@price, @timestamp, @payment, @exam, @owner)";
        private static readonly string GetBillQuery = "SELECT * FROM bill WHERE examination=@exam";

        public bool InsertBill(Bill bill)
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
                    Command.CommandText = InsertBillQuery;
                    Command.Parameters.AddWithValue("@price", bill.Price);
                    Command.Parameters.AddWithValue("@timestamp", DateTime.Now);
                    Command.Parameters.AddWithValue("@payment", bill.Payment);
                    Command.Parameters.AddWithValue("@exam", bill.Examination.Id);
                    Command.Parameters.AddWithValue("@owner", bill.Owner.Id);;

                    return Command.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception) { return false; }
        }

        public Bill? GetBillForExamination(Examination examination)
        {
            Connection = null;
            Command = null;
            Reader = null;

            Bill? bill = null;
            try
            {
                using (Connection = new MySqlConnection(MySqlUtils.ConnectionString))
                {
                    Connection.Open();
                    Command = Connection.CreateCommand();
                    Command.CommandText = GetBillQuery;
                    Command.Parameters.AddWithValue("@exam", examination.Id);
                    Reader = Command.ExecuteReader();

                    if (Reader.Read())
                    {
                        bill = new Bill()
                        {
                            Id = Reader.GetInt32("id"),
                            Timestamp = Reader.GetDateTime("timestamp"),
                            Price = Reader.GetDecimal("totalprice"),
                            Payment = Reader.GetString("payment"),
                            Examination = examination,
                            Owner = OwnerDao.GetById(Reader.GetInt32("user"))
                        };
                    }

                    Reader.Close();
                }
            }
            catch (Exception) { }

            return bill;
        }
    }
}
