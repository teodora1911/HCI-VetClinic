using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.Dao.MySqlDao;

namespace VetClinic.Dao
{
    public abstract class DaoFactory
    {
        public abstract IAdministratorDao Administrators { get; }
        public abstract IVeterinarianDao Veterinarians { get; }
        public abstract IMedicineDao Medicine { get; }
        public abstract IServiceDao Services { get; }
        public abstract IAppointmentDao Appointments { get; }
        public abstract IPetOwnerDao PetOwners { get; }
        public abstract IPetDao Pets { get; }
        public abstract ISpeciesBreedsDao SpeciesAndBreeds { get; }
        public abstract IExaminationDao Examinations { get; }

        public static DaoFactory Instance(DaoType type)
        {
            if (type.Equals(DaoType.MySql))
            {
                return new MySqlDaoFactory();
            }
            else throw new ArgumentException();
        }
    }

    public enum DaoType
    {
        MySql
    }
}
