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
    }
}
