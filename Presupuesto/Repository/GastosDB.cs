using Domain.Dtos.Cliente;
using Domain.Shared;
using Presupuesto.DataBase;
using System.Data;


namespace Presupuesto.Repository
{
    public class GastosDB : IGastosDB
    {
        private readonly IConnection _connection;
        public GastosDB(IConnection connection) 
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

            var conn = _connection.ObtenerConexion();

            IDbCommand command = conn.CreateCommand();
          
            command.CommandType = CommandType.Text;
            command.CommandText = @"SELECT * FROM Presupuesto where IdRubro=@idRubro AND IdPresupuesto=@idPresupuesto";

            var parameterIdRubro = command.CreateParameter();
            parameterIdRubro.ParameterName = "@idRubro";
            parameterIdRubro.Value = idRubro;

            var parameterIdPresupuesto = command.CreateParameter();
            parameterIdPresupuesto.ParameterName = "@idPresupuesto";
            parameterIdPresupuesto.Value = idPresupuesto;

            command.Parameters.Add(parameterIdRubro);
            command.Parameters.Add(parameterIdPresupuesto);

            conn.Open();
            IDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    gastoRubro = reader.GetDecimal(5);
                    puedeGastar = valorAGastar <= (reader.GetDecimal(4) - gastoRubro);
                }

                conn.Close();
                return new PuedeGastarResponse() { GastoRubro = gastoRubro, PuedeGastar = puedeGastar };
        }

        private void ActualizaGastoEnPresupuesto(decimal gastoRubro, decimal valorAGastar, string idRubro, int idPresupuesto)
        {
            var conn = _connection.ObtenerConexion();
            
            IDbCommand command = conn.CreateCommand();

            conn.Open();
            command.CommandText = @"UPDATE Presupuesto SET Gastado= @gastado WHERE (IdRubro= @idRubro AND IdPresupuesto= @idPresupuesto)";

            var parameterGastado = command.CreateParameter();
            parameterGastado.ParameterName = "@gastado";
            parameterGastado.Value = gastoRubro + valorAGastar;
            command.Parameters.Add(parameterGastado);

            var parameterIdRubro = command.CreateParameter();
            parameterIdRubro.ParameterName = "@idRubro";
            parameterIdRubro.Value = idRubro;
            command.Parameters.Add( parameterIdRubro);

            var parameteridPresupuesto = command.CreateParameter();
            parameteridPresupuesto.ParameterName = "@idPresupuesto";
            parameteridPresupuesto.Value = idPresupuesto;
            command.Parameters.Add(parameteridPresupuesto);

            command.ExecuteNonQuery();
            conn.Close();
        }

        public async Task<string> AgregarGasto(AgregarGastoRequest detalle)
        {
            ////Se generan ids de manera random
            //Random rnd = new Random();
            //var parteEntera = rnd.Next(10000000, 100000000);
            //var idGasto = detalle.IdRubro + parteEntera;

            ////Se verifica que se pueda hacer ese gasto
            //var puedeGastarResponse = this.PuedeGastar(detalle.IdRubro, detalle.Gasto, detalle.IdPresupuesto);
            //if (!puedeGastarResponse.PuedeGastar) { return "Excede el presupuesto estimado"; }

            ////Se inserta un gasto a la tabla de gastos
            //SqlConnection conn = _connection.ObtenerConexion();
            //using (SqlCommand cdmAgregaGasto = new SqlCommand())
            //{
            //    cdmAgregaGasto.Connection = conn;
            //    cdmAgregaGasto.CommandType = CommandType.Text;
            //    cdmAgregaGasto.CommandText = @"INSERT INTO Gastos(Id,IdPresupuesto,Gasto,Usuario,FechaCreacion, Mes, Anio) VALUES (@idGasto,@idPresupuesto,@gasto,@usuario,@fechaCreacion, @mes, @anio)";

            //    cdmAgregaGasto.Parameters.AddWithValue("@idGasto", idGasto);
            //    cdmAgregaGasto.Parameters.AddWithValue("@idPresupuesto", detalle.IdPresupuesto);
            //    cdmAgregaGasto.Parameters.AddWithValue("@gasto", detalle.Gasto);
            //    cdmAgregaGasto.Parameters.AddWithValue("@usuario", detalle.Usuario);
            //    cdmAgregaGasto.Parameters.AddWithValue("@fechaCreacion", DateTime.UtcNow.AddHours(-3));
            //    cdmAgregaGasto.Parameters.AddWithValue("@mes", DateTime.UtcNow.AddHours(-3).Month);
            //    cdmAgregaGasto.Parameters.AddWithValue("@anio", DateTime.UtcNow.AddHours(-3).Year);

            //    cdmAgregaGasto.ExecuteNonQuery();
            //}

            //conn.Close();

            ////Se actualiza gasto en el presupuesto
            //this.ActualizaGastoEnPresupuesto(puedeGastarResponse.GastoRubro,detalle.Gasto,detalle.IdRubro,detalle.IdPresupuesto);

            //return idGasto;
            return "";
            
        }

        public List<Gastos> ObtenerGastos(string command)
        {
            var conn = _connection.ObtenerConexion();
            IDbCommand commando = conn.CreateCommand();
                  
            commando.CommandText = command;

            conn.Open();
            IDataReader reader = commando.ExecuteReader();

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
            //Obtener el valor de lo gastado en presupuesto
            var puedeGastarResponse = this.PuedeGastar(gasto.IdRubro, 0, gasto.IdPresupuesto);
            
            //Obtener el gasto a eliminar
            var comando = string.Format("SELECT * FROM Gastos WHERE Id='{0}';", gasto.Id);
            var gastoARestar = this.ObtenerGastos(comando)?.FirstOrDefault()?.Gasto;

            //Se valida que el valor a restar sea menor o igual a lo gastado en presupuesto
            var sePuedeActualizarPresupuesto = gastoARestar <= puedeGastarResponse.GastoRubro;

            //Actualiza gasto en presupuesto
            if (sePuedeActualizarPresupuesto)
                this.ActualizaGastoEnPresupuesto(puedeGastarResponse.GastoRubro, -gastoARestar ?? 0, gasto.IdRubro, gasto.IdPresupuesto);
            else
                return "No se pudo eliminar el gasto. El monto del gasto a eliminar excede el gastado del presupuesto";

            //Eliminar gasto
            var conn = _connection.ObtenerConexion();

            conn.Open();

            IDbCommand command = conn.CreateCommand();

            command.CommandText = @"DELETE FROM Gastos WHERE Id = @id";

            var parameter = command.CreateParameter();
            parameter.ParameterName = "@id";
            parameter.Value = gasto.Id;
            command.Parameters.Add(parameter);

            command.ExecuteNonQuery();

            conn.Close();
            return "El gasto se eliminó correctamente.";
        }

        public async Task<string> ActualizaGasto(EditarGasto detalle)
        {
            //var puedeGastarResponse = this.PuedeGastar(detalle.IdRubro, detalle.Gasto, detalle.IdPresupuesto);
            //if (!puedeGastarResponse.PuedeGastar) { return "Excede el presupuesto estimado"; }

            ////se verifica si se debe sumar o restar en el presupuesto Gastado
            //var verifica = this.OperacionEnPresupuesto(detalle);

            ////Actualiza Presupuesto Gastado
            //this.ActualizaGastoEnPresupuestoPut(puedeGastarResponse.GastoRubro, detalle.IdPresupuesto, verifica);

            ////actualiza Tabla Gastos
            //using (SqlConnection conn = _connection.ObtenerConexion())
            //{
            //    using (SqlCommand cmd = new SqlCommand())
            //    {
            //        cmd.Connection = conn;
            //        cmd.CommandType = CommandType.Text;
            //        cmd.CommandText = @"UPDATE Gastos SET Gasto = @gasto WHERE Id = @id AND Usuario = @usuario AND IdPresupuesto = @idPresupuesto; ";

            //        cmd.Parameters.AddWithValue("@gasto", detalle.Gasto);
            //        cmd.Parameters.AddWithValue("@id", detalle.Id);
            //        cmd.Parameters.AddWithValue("@usuario", detalle.Usuario);
            //        cmd.Parameters.AddWithValue("@idPresupuesto", detalle.IdPresupuesto);

            //        //conn.Open();
            //        cmd.ExecuteNonQuery();

            //        conn.Close();
            //        return "El gasto se actualizó correctamente.";
            //    }
            //}

            return "";

        }

        private Operacion OperacionEnPresupuesto(EditarGasto detalle)
        {
            //// chequea si el gasto editado es mayor o menor al original
            //using (SqlConnection conn = _connection.ObtenerConexion())
            //{
            //    decimal gasto = 0;
            //    using (SqlCommand cmd = new SqlCommand())
            //    {
            //        cmd.Connection = conn;
            //        cmd.CommandType = CommandType.Text;
            //        cmd.CommandText = @"SELECT * FROM Gastos where Id=@Id";

            //        cmd.Parameters.AddWithValue("@Id", detalle.Id);

            //        SqlDataReader reader = cmd.ExecuteReader();

            //        while(reader.Read())
            //        {
            //            //gasto original
            //            gasto = reader.GetDecimal("Gasto");
            //        }
            //    }
            //    //compara los 2 gastos para saber si se debe restar o sumar en la tabla Presupuesto
            //    if (gasto < detalle.Gasto)
            //    {
            //        conn.Close();
            //        decimal valor = detalle.Gasto - gasto;
            //        return new Operacion() { op= "+", diferencia = valor };
            //    }
            //    if (gasto > detalle.Gasto)
            //    {
            //        conn.Close();
            //        decimal valor = gasto - detalle.Gasto;
            //        return new Operacion() { op = "-", diferencia = valor};
            //    }
            //    else
            //    {
            //        conn.Close();
            //        return new Operacion() { op = "=", diferencia= 0};
            //    }
            //}

            return new Operacion();
        }

        private void ActualizaGastoEnPresupuestoPut(decimal valorActual, int idPresupuesto, Operacion operacion)
        {
            var conn = _connection.ObtenerConexion();

            IDbCommand command = conn.CreateCommand();

            conn.Open();

            command.CommandText = @"UPDATE Presupuesto SET Gastado= @gastado WHERE IdPresupuesto = @idPresupuesto;";

            var parameter = command.CreateParameter();
            parameter.ParameterName = "@idPresupuesto";
            parameter.Value = idPresupuesto;
            command.Parameters.Add(parameter);

            if (operacion.op == "+")
            {
                var parameterGastado = command.CreateParameter();
                parameterGastado.ParameterName = "@gastado";
                parameterGastado.Value = valorActual + operacion.diferencia;
                command.Parameters.Add( parameterGastado );
            }
            if (operacion.op == "-")
            {
                var parameterGastado = command.CreateParameter();
                parameterGastado.ParameterName = "@gastado";
                parameterGastado.Value = valorActual - operacion.diferencia;
                command.Parameters.Add( parameterGastado ); 
            }
            else if (operacion.op == "=")
            {
                var parameterGastado = command.CreateParameter();
                parameterGastado.ParameterName = "@gastado";
                parameterGastado.Value = valorActual;
                command.Parameters.Add(parameterGastado);
            }
               command.ExecuteNonQuery();

               conn.Close();
        }
    }

    public interface IGastosDB
    {
        Task<List<Gastos>> GastosPorMesAño(string mesAño);
        Task<string> AgregarGasto(AgregarGastoRequest detalle);
        List<Gastos> ObtenerGastos(string command);
        Task<string> EliminarGasto(EliminaGasto gasto);
        Task<string> ActualizaGasto(EditarGasto detalle);

    }
}
