using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.Models.Entities;

namespace VetClinic.Dao
{
    public interface IPetOwnerDao : IGenericSearchDao<PetOwner, int>
    {
    }
}
