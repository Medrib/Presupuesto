using Domain.Dtos.Cliente;
using Presupuesto.DataBase;
using System.Data;
using System.Data.SqlClient;

namespace Presupuesto.Repository
{
    public class ValidacionUsuarioDB
    {
        public async Task<bool> ValidarUsuario(string user, string password)
        {
            //GetUser datosUsuario = new GetUser();
            //{
            //    user = user;
            //    password = password;
            //}

            bool res = TestValidarUsuario(user, password);
            
            if (res)
            {
               return ResponseValidarTrue();
            }
            else 
            {
               return ResponseValidarFalse();
            }
        }
        public static bool TestValidarUsuario(string user, string password)
        {
            using (SqlConnection connection = Connection.ObtenerConexion())
            {
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
      
        public static bool ResponseValidarTrue()
        {
            return true;
        }
        public static bool ResponseValidarFalse()
        {
            return false;
        }
    }
}
