using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
namespace KaraokeManagementSystem
{
    internal class DBConnection
    {
        public static SqlConnection GetConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["KaraokeManagementSystem.Properties.Settings.KaraokeConnectionString"].ConnectionString;
            return new SqlConnection(connectionString);
        }
    }

}
