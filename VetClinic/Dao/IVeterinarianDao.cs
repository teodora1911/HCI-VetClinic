using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetClinic.Models.Entities;

namespace VetClinic.Dao
{
    public interface IVeterinarianDao : IGenericSearchDao<Veterinarian, int>
    {
        Veterinarian GetByUsernameAndPassword(string username, string password);
        List<string> GetTitles();
//        List<Veterinarian> GetByTitle(string title);
//        List<Veterinarian> GetByTitleAndQuery(string title, string query);
    }
}
