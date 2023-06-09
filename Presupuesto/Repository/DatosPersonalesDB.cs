using Presupuesto.DataBase;
using Presupuesto.DataBase.Models;
using System.Data.SqlClient;
using System.Net.NetworkInformation;

namespace Presupuesto.Repository
{
    public class DatosPersonalesDB
    {
        public async Task<DatosPersonales> DatosPersonalesByDni(int dni)
        {
            DatosPersonales clienteData = new DatosPersonales();
            using (SqlConnection connection = BDPresupuesto.ObtenerConexion())
            {
                SqlCommand command = new SqlCommand(
                    string.Format("Select Dni, Nombre, Apellido, Domicilio, Fecha_Nacimiento from DatosPersonales where Dni like {0}", dni),
                      connection
                );

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    clienteData.Dni = reader.GetInt32(0);
                    clienteData.Nombre = reader.GetString(1);
                    clienteData.Apellido = reader.GetString(2);
                    clienteData.Domicilio = reader.GetString(3);
                    clienteData.Fecha_Nacimiento = reader.GetDateTime(4);
                }

                connection.Close();
                return clienteData;
            }
        }
    }
}
  