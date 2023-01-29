using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetClinic.Dao
{
    public interface IGenericSearchDao<T, ID> : IGenericDao<T, ID>
    {
        List<T> GetBySearchQuery(string query);
    }
}
