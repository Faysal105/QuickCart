using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QuickCart
{
    public static class DataAccess
    {
        private static string connectionString =
            @"Data Source=.\SQLEXPRESS;Initial Catalog=cart_management;Integrated Security=True";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
