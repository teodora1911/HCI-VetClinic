using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetClinic.Dao.MySqlDao
{
    public class MySqlDaoFactory : DaoFactory
    {
        private MySqlAdministratorDao? MySqlAdmin;
        private MySqlVeterinaranDao? MySqlVeterinaran;
        private MySqlMedicineDao? MySqlMedicine;
        private MySqlServiceDao? MySqlService;
        private MySqlAppointmentDao? MySqlAppointment;
        private MySqlPetOwnerDao? MySqlPetOwner;
        private MySqlPetDao? MySqlPet;
        private MySqlSpeciesBreedsDao? MySqlSpeciesAndBreeds;
        private MySqlExaminationDao? MySqlExamination;

        public override IAdministratorDao Administrators
        {
            get
            {
                if (MySqlAdmin == null)
                    MySqlAdmin = new();
                return MySqlAdmin;
            }
        }

        public override IVeterinarianDao Veterinarians
        {
            get
            {
                if(MySqlVeterinaran == null)
                    MySqlVeterinaran = new();
                return MySqlVeterinaran;
            }
        }

        public override IMedicineDao Medicine
        {
            get
            {
                if (MySqlMedicine == null)
                    MySqlMedicine = new();
                return MySqlMedicine;
            }
        }

        public override IServiceDao Services
        {
            get
            {
                if(MySqlService == null)
                    MySqlService = new();
                return MySqlService;
            }
        }

        public override IAppointmentDao Appointments
        {
            get
            {
                if(MySqlAppointment == null)
                    MySqlAppointment = new();
                return MySqlAppointment;
            }
        }

        public override IPetOwnerDao PetOwners
        {
            get
            {
                if(MySqlPetOwner == null)
                    MySqlPetOwner = new();
                return MySqlPetOwner;
            }
        }

        public override IPetDao Pets
        {
            get
            {
                if(MySqlPet == null)
                    MySqlPet = new();
                return MySqlPet;
            }
        }

        public override ISpeciesBreedsDao SpeciesAndBreeds
        {
            get
            {
                if (MySqlSpeciesAndBreeds == null)
                    MySqlSpeciesAndBreeds = new();
                return MySqlSpeciesAndBreeds;
            }
        }

        public override IExaminationDao Examinations
        {
            get
            {
                if(MySqlExamination == null)
                    MySqlExamination = new();
                return MySqlExamination;
            }
        }
    }
}
