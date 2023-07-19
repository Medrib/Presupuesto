using Domain.Dtos.Cliente;
using Domain.Shared;
using Presupuesto.DataBase;
using System.Data;
using System.Data.SqlClient;

namespace Presupuesto.Repository
{
    public class GastosDB
    {
        //public async Task<List<Gastos>> ObtenerGastosTotales()
        //{
        //    using (SqlConnection connection = Connection.ObtenerConexion())
        //    {
        //        SqlCommand command = new SqlCommand(
        //          string.Format("SELECT Id, Valor, Consumidor, Fecha FROM Gastos"),
        //            connection
        //      );

        //        SqlDataReader reader = command.ExecuteReader();

        //        var gastos = new List<Gastos>();

        //        while (reader.Read())
        //        {
        //            var gastoData = new Gastos()
        //            {
        //               Id = reader.GetString(0),
        //               Valor = reader.GetDecimal(1),
        //               Consumidor = reader.GetInt32(2),
        //               Fecha = reader.GetDateTime(3)
        //            };
        //            gastos.Add(gastoData);
        //        }

        //        connection.Close();

        //        return gastos;

        //    }
        //}

        //public async Task<List<Gastos>> GastosPorProducto(string idRubro)
        //{
        //    var gastos = new List<Gastos>();
        //    //idRubro example: IND
        //    using (SqlConnection connection = Connection.ObtenerConexion())
        //    {
        //        SqlCommand command = new SqlCommand(
        //            string.Format("SELECT Id, Valor, Consumidor, Fecha FROM Gastos"),
        //              connection
        //        );

        //        SqlDataReader reader = command.ExecuteReader();
        //        var gasto = new Gastos();

        //        while (reader.Read())
        //        {
        //            if(reader.GetString(0).Contains(idRubro.ToUpper()))
        //            {
        //                gasto.Id = reader.GetString(0);
        //                gasto.Valor = reader.GetDecimal(1);
        //                gasto.Consumidor = reader.GetInt32(2);
        //                gasto.Fecha = reader.GetDateTime(4);
        //                gastos.Add(gasto);
        //            }

        //        }

        //        connection.Close();
        //        return gastos;
        //    }
        //}

        public async Task<List<Gastos>> GastosPorMesAño(string mesAño)
        {

            var fecha = Functions.mesAñoIntParse(mesAño);
       
            using (SqlConnection connection = Connection.ObtenerConexion())
            {
                SqlCommand command = new SqlCommand(
                    string.Format("SELECT * FROM Gastos WHERE Mes={0} AND Anio={1};", fecha.Mes, fecha.Año),
                      connection
                );

                SqlDataReader reader = command.ExecuteReader();
                                 
                var gastoMes = new List<Gastos>();

                while (reader.Read())
                {
                    var gasto = new Gastos()
                    {
                        Id = reader.GetString(0),
                        IdPresupuesto = reader.GetInt32(1),
                        Gasto = reader.GetDecimal(2),
                        Usuario = reader.GetString(3),
                        FechaCreacion = reader.GetDateTime(4),
                        Mes = reader.GetInt32(5),
                        Año = reader.GetInt32(6),

                    };
                        gastoMes.Add(gasto);                        
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
                cmdPresupuesto.CommandText = @"SELECT IdPresupuesto, IdRubro, Rubro, Usuario, Presupuesto, Gastado, FechaDeCreacion, Mes, Anio FROM Presupuesto where IdRubro=@idRubro";
                cmdPresupuesto.Parameters.AddWithValue("@idRubro", idRubro);

                SqlDataReader reader = cmdPresupuesto.ExecuteReader();

                while (reader.Read())
                {
                    gastoRubro = reader.GetDecimal("Gastado");
                    puedeGastar = valorAGastar <= (reader.GetDecimal("Presupuesto") - gastoRubro);
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
            var puedeGastarResponse = this.PuedeGastar(detalle.IdRubro, detalle.Gasto);
            if (!puedeGastarResponse.PuedeGastar) { return "Excede el presupuesto estimado"; }

            //Se inserta un gasto a la tabla de gastos
            SqlConnection conn = Connection.ObtenerConexion();
            using (SqlCommand cdmAgregaGasto = new SqlCommand())
            {
                cdmAgregaGasto.Connection = conn;
                cdmAgregaGasto.CommandType = CommandType.Text;
                cdmAgregaGasto.CommandText = @"INSERT INTO Gastos(Id,IdPresupuesto,Gasto,Usuario,FechaCreacion, Mes, Anio) VALUES (@idGasto,@idPresupuesto,@gasto,@usuario,@fechaCreacion, @mes, @anio)";

                cdmAgregaGasto.Parameters.AddWithValue("@idGasto", idGasto);
                cdmAgregaGasto.Parameters.AddWithValue("@idPresupuesto", detalle.IdPresupuesto);
                cdmAgregaGasto.Parameters.AddWithValue("@gasto", detalle.Gasto);
                cdmAgregaGasto.Parameters.AddWithValue("@usuario", detalle.Usuario);
                cdmAgregaGasto.Parameters.AddWithValue("@fechaCreacion", DateTime.UtcNow.AddHours(-3));
                cdmAgregaGasto.Parameters.AddWithValue("@mes", DateTime.UtcNow.AddHours(-3).Month);
                cdmAgregaGasto.Parameters.AddWithValue("@anio", DateTime.UtcNow.AddHours(-3).Year);

                cdmAgregaGasto.ExecuteNonQuery();
            }

            conn.Close();

            //Se actualiza gasto en el presupuesto
           // this.ActualizaGastoEnPresupuesto(puedeGastarResponse.GastoRubro,detalle.Gasto,detalle.IdRubro,detalle.IdPresupuesto);
            
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
