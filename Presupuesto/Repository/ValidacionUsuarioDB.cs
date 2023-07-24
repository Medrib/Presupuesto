using Presupuesto.DataBase;
using System.Data;
using System.Data.SqlClient;

namespace Presupuesto.Repository
{
    public class ValidacionUsuarioDB
    {
        private readonly Connection _connection;
        public ValidacionUsuarioDB(Connection connection)
        {
            _connection = connection;
        }
        public async Task<bool> ValidarUsuario(string user, string password)
        {
            SqlConnection connection = _connection.ObtenerConexion();
            SqlCommand cmd = new SqlCommand();

            cmd.Connection = connection;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"SELECT Usr, Psw FROM Users WHERE Usr = @User";

            cmd.Parameters.AddWithValue("@User", user);

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                return reader.GetString("Psw") == password;
            }

            connection.Close();

            return false;
        }
    }
}
