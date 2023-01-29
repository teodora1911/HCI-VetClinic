using System.Configuration;

namespace VetClinic.Utils
{
    public sealed class MySqlUtils
    {
        public static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["hci"].ConnectionString;
    }
}
