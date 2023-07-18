using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Presupuesto.DataBase
{
    public class Connection
    {
        public static SqlConnection ObtenerConexion()
        {
            SqlConnection Conn = new SqlConnection("Data Source=NBCORAR860\\SQLEXPRESS; Initial Catalog=Cliente;User Id=sa; Password=Mayo2020");
            Conn.Open();

            return Conn;
        }
    }
}
