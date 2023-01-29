using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetClinic.Dao
{
    public interface IGenericDao<T, ID>
    {
        T GetById(ID id);
        List<T> GetAll();
        int Create(T entity); // -1 for server error, 0 for client error, 1 success
        bool Update(T entity);
        bool DeleteById(ID id);
    }
}
