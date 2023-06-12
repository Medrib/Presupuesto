using Domain.Dtos.Cliente;
using Presupuesto.DataBase;
using System.Data;
using System.Data.SqlClient;
using System.Net;

namespace Presupuesto.Repository
{
    public class GastosDB
    {
        public async Task<List<Gastos>> GastosPorMes()
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
    }
}
