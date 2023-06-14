using Azure;
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

        public async Task<List<Gastos>> GastosPorMes(int mes)
        {
            var gastoMes = new List<Gastos>();
       
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
                    if(reader.GetDateTime(3).Month == mes && mes <= 12 && mes >= 1)
                    {
                        gasto.Id = reader.GetString(0);
                        gasto.Valor = reader.GetDecimal(1);
                        gasto.Consumidor = reader.GetInt32(2);
                        gasto.Fecha = reader.GetDateTime(3);
                        gastoMes.Add(gasto);

                    }
                        
                }
                connection.Close();
                return gastoMes;
            }
        }

        private PuedeGastarResponse PuedeGastar(string idRubro, decimal valorAGastar)
        {
            decimal gastoRubro = 0;
            var puedeGastar = false;

            using (SqlConnection conn = Connection.ObtenerConexion())
            {
                SqlCommand cmdPresupuesto = new SqlCommand();
                cmdPresupuesto.Connection = conn;
                cmdPresupuesto.CommandType = CommandType.Text;
                cmdPresupuesto.CommandText = @"SELECT IdPresupuesto, IdRubro, Rubro, Responsable, Estimado, GastoRubro, FechaInicio, FechaFin FROM Presupuesto where IdRubro=@idRubro";
                cmdPresupuesto.Parameters.AddWithValue("@idRubro", idRubro);

                SqlDataReader reader = cmdPresupuesto.ExecuteReader();

                while (reader.Read())
                {
                    gastoRubro = reader.GetDecimal("GastoRubro");
                    puedeGastar = valorAGastar <= (reader.GetDecimal("Estimado") - gastoRubro);
                }

                conn.Close();
                return new PuedeGastarResponse() {GastoRubro = gastoRubro, PuedeGastar = puedeGastar};
            }
        }

        private void ActualizaGastoEnPresupuesto(decimal gastoRubro, decimal valorAGastar, string idRubro, int idPresupuesto)
        {
            using (SqlConnection conn = Connection.ObtenerConexion())
            {
                SqlCommand cmdActualizaPresupuesto = new SqlCommand();
                cmdActualizaPresupuesto.Connection = conn;

                cmdActualizaPresupuesto.Connection = conn;
                cmdActualizaPresupuesto.CommandText = @"UPDATE Presupuesto SET GastoRubro= @gastoRubro WHERE (IdRubro= @idRubro AND IdPresupuesto= @idPresupuesto)";
                cmdActualizaPresupuesto.Parameters.AddWithValue("@gastoRubro", gastoRubro + valorAGastar);
                cmdActualizaPresupuesto.Parameters.AddWithValue("@idRubro", idRubro);
                cmdActualizaPresupuesto.Parameters.AddWithValue("@idPresupuesto", idPresupuesto);

                cmdActualizaPresupuesto.ExecuteNonQuery();

                conn.Close();  
            }
        }

        public async Task<string> AgregarGasto(AgregarGastoRequest detalle)
        {
            //Se generan ids de manera random
            Random rnd = new Random();
            var parteEntera = rnd.Next(10000000, 100000000);
            var idGasto = detalle.IdRubro + parteEntera;

            //Se verifica que se pueda hacer ese gasto
            var puedeGastarResponse = this.PuedeGastar(detalle.IdRubro, detalle.Valor);
            if (!puedeGastarResponse.PuedeGastar) { return "Excede el presupuesto estimado"; }

            //Se inserta un gasto a la tabla de gastos
            SqlConnection conn = Connection.ObtenerConexion();
            using (SqlCommand cdmAgregaGasto = new SqlCommand())
            {
                cdmAgregaGasto.Connection = conn;
                cdmAgregaGasto.CommandType = CommandType.Text;
                cdmAgregaGasto.CommandText = @"INSERT INTO Gastos(Id,IdPresupuesto,Valor,Consumidor,Fecha) VALUES (@idGasto,@idPresupuesto,@valor,@consumidor,@fecha)";

                cdmAgregaGasto.Parameters.AddWithValue("@idGasto", idGasto);
                cdmAgregaGasto.Parameters.AddWithValue("@idPresupuesto", detalle.IdPresupuesto);
                cdmAgregaGasto.Parameters.AddWithValue("@valor", detalle.Valor);
                cdmAgregaGasto.Parameters.AddWithValue("@consumidor", detalle.Consumidor);
                cdmAgregaGasto.Parameters.AddWithValue("@fecha", DateTime.UtcNow.AddHours(-3));

                cdmAgregaGasto.ExecuteNonQuery();
            }

            conn.Close();

            //Se actualiza gasto en el presupuesto
            this.ActualizaGastoEnPresupuesto(puedeGastarResponse.GastoRubro,detalle.Valor,detalle.IdRubro,detalle.IdPresupuesto);
            
            return idGasto;
            
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
