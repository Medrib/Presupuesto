using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Presupuesto.DataBase
{
    public class BDPresupuesto
    {
        public static SqlConnection ObtenerConexion()
        {
            SqlConnection Conn = new SqlConnection("Data Source=MEDRANO; Initial Catalog=Cliente;User Id=Admin; Password=Junio2023!");
            Conn.Open();

            return Conn;
        }
    }
}
