﻿using Domain.Dtos.Cliente;
using Domain.Shared;
using System.Data;
using System.Data.SqlClient;
using Connection = Presupuesto.DataBase.Connection;

namespace Presupuesto.Repository
{
    public class GastosDB
    {
        private readonly Connection _connection;
        public GastosDB(Connection connection) 
        {
            _connection = connection;
        }
        public async Task<List<Gastos>> GastosPorMesAño(string mesAño)
        {

            var fecha = Functions.mesAñoIntParse(mesAño);

            var command = string.Format("SELECT * FROM Gastos WHERE Mes={0} AND Anio={1};", fecha.Mes, fecha.Año);

            return this.ObtenerGastos(command);
        }

        private PuedeGastarResponse PuedeGastar(string idRubro, decimal valorAGastar, int idPresupuesto)
        {
            decimal gastoRubro = 0;
            var puedeGastar = false;

            using (SqlConnection conn = _connection.ObtenerConexion())
            {
                SqlCommand cmdPresupuesto = new SqlCommand();
                cmdPresupuesto.Connection = conn;
                cmdPresupuesto.CommandType = CommandType.Text;
                cmdPresupuesto.CommandText = @"SELECT * FROM Presupuesto where IdRubro=@idRubro AND IdPresupuesto=@idPresupuesto";

                cmdPresupuesto.Parameters.AddWithValue("@idRubro", idRubro);
                cmdPresupuesto.Parameters.AddWithValue("@IdPresupuesto", idPresupuesto);

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
            using (SqlConnection conn = _connection.ObtenerConexion())
            {
                SqlCommand cmdActualizaPresupuesto = new SqlCommand();
                cmdActualizaPresupuesto.Connection = conn;

                cmdActualizaPresupuesto.Connection = conn;
                cmdActualizaPresupuesto.CommandText = @"UPDATE Presupuesto SET Gastado= @gastado WHERE (IdRubro= @idRubro AND IdPresupuesto= @idPresupuesto)";
                cmdActualizaPresupuesto.Parameters.AddWithValue("@gastado", gastoRubro + valorAGastar);
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
            var puedeGastarResponse = this.PuedeGastar(detalle.IdRubro, detalle.Gasto, detalle.IdPresupuesto);
            if (!puedeGastarResponse.PuedeGastar) { return "Excede el presupuesto estimado"; }

            //Se inserta un gasto a la tabla de gastos
            SqlConnection conn = _connection.ObtenerConexion();
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
            this.ActualizaGastoEnPresupuesto(puedeGastarResponse.GastoRubro,detalle.Gasto,detalle.IdRubro,detalle.IdPresupuesto);
            
            return idGasto;
            
        }

        public List<Gastos> ObtenerGastos(string command)
        {
            SqlConnection conn = _connection.ObtenerConexion();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = command;

            SqlDataReader reader = cmd.ExecuteReader();

            var gastos = new List<Gastos>();
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
                gastos.Add(gasto);
            }
            conn.Close();
            return gastos;
        }

        public async Task<string> EliminarGasto(EliminaGasto gasto)
        {

            SqlConnection conn = _connection.ObtenerConexion();
            SqlCommand cmd = new SqlCommand();

            //Obtener el valor de lo gastado en presupuesto
            var puedeGastarResponse = this.PuedeGastar(gasto.IdRubro, 0, gasto.IdPresupuesto);

            //Obtener el gasto a eliminar
            var command = string.Format("SELECT * FROM Gastos WHERE Id={0};",gasto.Id);
            var gastoARestar = this.ObtenerGastos(command)?.FirstOrDefault()?.Gasto;

            //Se valida que el valor a restar sea menor o igual a lo gastado en presupuesto
            var sePuedeActualizarPresupuesto = gastoARestar <= puedeGastarResponse.GastoRubro;

            //Actualiza gasto en presupuesto
            if (sePuedeActualizarPresupuesto)
                this.ActualizaGastoEnPresupuesto(puedeGastarResponse.GastoRubro, -gastoARestar ?? 0, gasto.IdRubro, gasto.IdPresupuesto);
            else
                return "No se pudo eliminar el gasto. El monto del gasto a eliminar excede el gastado del presupuesto";

            //Eliminar gasto
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"DELETE FROM Gastos WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", gasto.Id);
            cmd.ExecuteNonQuery();
            conn.Close();
            return "El gasto se eliminó correctamente.";
        }

        public async Task<string> ActualizaGasto(EditarGasto detalle)
        {
            var puedeGastarResponse = this.PuedeGastar(detalle.IdRubro, detalle.Gasto, detalle.IdPresupuesto);
            if (!puedeGastarResponse.PuedeGastar) { return "Excede el presupuesto estimado"; }

            //se verifica si se debe sumar o restar en el presupuesto Gastado
            var verifica = this.OperacionEnPresupuesto(detalle);

            //Actualiza Presupuesto Gastado
            this.ActualizaGastoEnPresupuestoPut(puedeGastarResponse.GastoRubro, detalle.IdPresupuesto, verifica);

            //actualiza Tabla Gastos
            using (SqlConnection conn = _connection.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"UPDATE Gastos SET Gasto = @gasto WHERE Id = @id AND Usuario = @usuario AND IdPresupuesto = @idPresupuesto; ";

                    cmd.Parameters.AddWithValue("@gasto", detalle.Gasto);
                    cmd.Parameters.AddWithValue("@id", detalle.Id);
                    cmd.Parameters.AddWithValue("@usuario", detalle.Usuario);
                    cmd.Parameters.AddWithValue("@idPresupuesto", detalle.IdPresupuesto);

                    //conn.Open();
                    cmd.ExecuteNonQuery();

                    conn.Close();
                    return "El gasto se actualizó correctamente.";
                }
            }

        }

        private Operacion OperacionEnPresupuesto(EditarGasto detalle)
        {
            // chequea si el gasto editado es mayor o menor al original
            using (SqlConnection conn = _connection.ObtenerConexion())
            {
                decimal gasto = 0;
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"SELECT * FROM Gastos where Id=@Id";

                    cmd.Parameters.AddWithValue("@Id", detalle.Id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while(reader.Read())
                    {
                        //gasto original
                        gasto = reader.GetDecimal("Gasto");
                    }
                }
                //compara los 2 gastos para saber si se debe restar o sumar en la tabla Presupuesto
                if (gasto < detalle.Gasto)
                {
                    conn.Close();
                    decimal valor = detalle.Gasto - gasto;
                    return new Operacion() { op= "+", diferencia = valor };
                }
                if (gasto > detalle.Gasto)
                {
                    conn.Close();
                    decimal valor = gasto - detalle.Gasto;
                    return new Operacion() { op = "-", diferencia = valor};
                }
                else
                {
                    conn.Close();
                    return new Operacion() { op = "=", diferencia= 0};
                }
            }
        }

        private void ActualizaGastoEnPresupuestoPut(decimal valorActual, int idPresupuesto, Operacion operacion)
        {
            using (SqlConnection conn = _connection.ObtenerConexion())
            {
                SqlCommand cmdActualizaPresupuesto = new SqlCommand();
                cmdActualizaPresupuesto.Connection = conn;

                cmdActualizaPresupuesto.Connection = conn;
                cmdActualizaPresupuesto.CommandText = @"UPDATE Presupuesto SET Gastado= @gastado WHERE IdPresupuesto = @idPresupuesto;";
                cmdActualizaPresupuesto.Parameters.AddWithValue("@idPresupuesto", idPresupuesto);
                if(operacion.op == "+") 
                {
                    cmdActualizaPresupuesto.Parameters.AddWithValue("@gastado", valorActual + operacion.diferencia);
                }
                if(operacion.op == "-")
                {
                    cmdActualizaPresupuesto.Parameters.AddWithValue("@gastado", valorActual - operacion.diferencia);
                }
                else if(operacion.op == "=")
                {
                    cmdActualizaPresupuesto.Parameters.AddWithValue("@gastado", valorActual);
                }

                cmdActualizaPresupuesto.ExecuteNonQuery();

                conn.Close();
            }
        }
    }
}
