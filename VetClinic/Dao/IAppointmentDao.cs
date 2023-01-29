using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.Models.Entities;

namespace VetClinic.Dao
{
    public interface IAppointmentDao : IGenericDao<Appointment, int>
    {
        List<Appointment> GetByScheduled(bool scheduled);
        List<Appointment> GetByVetNameSurname(string query);
        List<Appointment> GetByVetId(int id);
        List<Appointment> GetByOwnerFullName(string query);
        bool Schedule(Appointment appointment, string address);
        List<Appointment> GetBySpecs(string owner, string vet, bool scheduled);
    }
}
