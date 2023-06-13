using Domain.Dtos.Cliente;
using Presupuesto.DataBase;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;

namespace Presupuesto.Repository
{
    public class GastosDB
    {
        public async Task<List<Gastos>> ObtenerGastosTotales()
        {
            var gastos = new List<Gastos>();
            using (SqlConnection connection = Connection.ObtenerConexion())
            {
                SqlCommand command = new SqlCommand(
                  string.Format("SELECT Id, Valor, Consumidor, Fecha FROM Gastos"),
                    connection
              );

                SqlDataReader reader = command.ExecuteReader();

                var gastoData = new Gastos();
                while (reader.Read())
                {
                    gastoData.Id = reader.GetString(0);
                    gastoData.Valor = reader.GetDecimal(1);
                    gastoData.Consumidor = reader.GetInt32(2);
                    gastoData.Fecha = reader.GetDateTime(3);
                    gastos.Add(gastoData);
                }

                connection.Close();

                return gastos;

            }
        }

        public async Task<List<Gastos>> GastosPorProducto(string idRubro)
        {
            var gastos = new List<Gastos>();
            //idRubro example: IND
            using (SqlConnection connection = Connection.ObtenerConexion())
            {
                SqlCommand command = new SqlCommand(
                    string.Format("SELECT Id, Valor, Consumidor, Fecha FROM Gastos"),
                      connection
                );

                SqlDataReader reader = command.ExecuteReader();
                var gasto = new Gastos();

                while (reader.Read())
                {
                    if(reader.GetString(0).Contains(idRubro.ToUpper()))
                    {
                        gasto.Id = reader.GetString(0);
                        gasto.Valor = reader.GetDecimal(1);
                        gasto.Consumidor = reader.GetInt32(2);
                        gasto.Fecha = reader.GetDateTime(4);
                        gastos.Add(gasto);
                    }

                }

                connection.Close();
                return gastos;
            }
        }

        public async Task<string> AgregarGasto(AgregarGastoRequest detalle)
        {
            var gastos = new List<Gastos>();
            Random rnd = new Random();
            var parteEntera = rnd.Next(10000000, 100000000);
            var idGasto = detalle.IdRubro + parteEntera;
            using (SqlConnection conn = Connection.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"INSERT INTO Gastos(Id,Valor,Consumidor,Fecha) VALUES (@idGasto,@valor,@consumidor,@fecha)";

                    cmd.Parameters.AddWithValue("@idGasto", idGasto);
                    cmd.Parameters.AddWithValue("@valor", detalle.Valor);
                    cmd.Parameters.AddWithValue("@consumidor", detalle.Consumidor);
                    cmd.Parameters.AddWithValue("@fecha", DateTime.UtcNow.AddHours(-3));

                    //conn.Open();
                    cmd.ExecuteNonQuery();

                    conn.Close();
                    return idGasto;
                }
            }
        }

        public async Task<string> EliminarGasto(string IdGasto)
        {
            using (SqlConnection conn = Connection.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"DELETE FROM Gastos WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@id", IdGasto);

                    //conn.Open();
                    cmd.ExecuteNonQuery();

                    conn.Close();
                    return "El gasto se eliminó correctamente.";
                }
            }
        }
    }
}
