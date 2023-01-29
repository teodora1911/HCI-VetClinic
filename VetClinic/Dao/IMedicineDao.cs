using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.Models.Entities;

namespace VetClinic.Dao
{
    public interface IMedicineDao : IGenericSearchDao<Medicine, int>
    {
        List<string> GetTypes();
        List<Medicine> GetByType(string type);
        List<Medicine> GetByNameAndType(string name, string type);
    }
}
