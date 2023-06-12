using Domain.Dtos.Cliente;
using Presupuesto.DataBase;
using System.Data;
using System.Data.SqlClient;
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

        public async Task<List<Gastos>> GastosPorProducto(string idProducto)
        {
            var gastos = new List<Gastos>();
            
            using (SqlConnection connection = Connection.ObtenerConexion())
            {
                SqlCommand command = new SqlCommand(
                    string.Format("Select Id, Valor, Consumidor, Fecha from Gastos where Id like {0}", idProducto),
                      connection
                );

                SqlDataReader reader = command.ExecuteReader();
                var gasto = new Gastos();

                while (reader.Read())
                {
                    gasto.Id = reader.GetString(0);
                    gasto.Valor = reader.GetDecimal(1);
                    gasto.Consumidor = reader.GetInt32(2);
                    gasto.Fecha = reader.GetDateTime(4);

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
            using (SqlConnection connection = Connection.ObtenerConexion())
            {
                SqlCommand command = new SqlCommand(
                    string.Format("INSERT INTO Gastos(Id,Valor,Consumidor,Fecha) VALUES ({0},{1},{2},{4})", idGasto, detalle.Valor, detalle.Consumidor, DateTime.Now.AddHours(-3)),
                      connection
                );

                connection.Close();
                return idGasto;
            }
        }
    }
}
