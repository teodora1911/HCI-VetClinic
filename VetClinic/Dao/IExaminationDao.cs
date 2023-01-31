using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.Models.Entities;

namespace VetClinic.Dao
{
    public interface IExaminationDao : IGenericDao<Examination, int>
    {
        Examination GetByAppointmentId(int appointmentId);
        List<Examination> GetAllFromVeterinarian(int veterinarianId);
        List<Examination> GetAllFromVetAndSearchPet(int vet, string search, bool completed);
        List<ExaminationService> GetAllSevicesFromExamination(int id);
        List<Prescription> GetAllPrescriptionsFromExamination(int id);
        bool AddService(ExaminationService entity);
        bool AddPrescription(Prescription prescription);
        bool UpdateService(ExaminationService examinationService);
        bool UpdatePrescription(Prescription prescription);
    }
}
