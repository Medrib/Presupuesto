using System.Data;
using System.Data.SqlClient;

namespace Presupuesto.DataBase
{
    public interface IConnection
    {
        IDbConnection ObtenerConexion();
    }
    public class Connection : IConnection
    {
        public IDbConnection ObtenerConexion()
        {
            //SqlConnection Conn = new SqlConnection("Data Source=NBCORAR860\\SQLEXPRESS; Initial Catalog=Cliente;User Id=sa; Password=Mayo2020");
            SqlConnection Conn = new SqlConnection("Data Source=MEDRANO; Initial Catalog=Cliente;User Id=Admin; Password=Julio2023!");
            //Conn.Open();

            return Conn;
        }
    }
}
