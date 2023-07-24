using System.Data.SqlClient;

namespace Presupuesto.DataBase
{
    public interface IConnection
    {
        SqlConnection ObtenerConexion();
    }
    public class Connection : IConnection
    {
        public SqlConnection ObtenerConexion()
        {
            SqlConnection Conn = new SqlConnection("Data Source=NBCORAR860\\SQLEXPRESS; Initial Catalog=Cliente;User Id=sa; Password=Mayo2020");
            Conn.Open();

            return Conn;
        }
    }
}
