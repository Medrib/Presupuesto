using Domain.Dtos.Cliente;
using Domain.Shared;
using Presupuesto.DataBase;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Metadata;


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
            bool eliminar = false;
            return this.ObtenerGastos(command, eliminar);
        }

        public PuedeGastarResponse PuedeGastar(string idRubro, decimal valorAGastar, int idPresupuesto)
        {
            decimal gastoRubro = 0;
            var puedeGastar = false;

            var conn = _connection.ObtenerConexion();

            IDbCommand command = conn.CreateCommand();
          
            command.CommandType = CommandType.Text;
            command.CommandText = @"SELECT * FROM Presupuesto where IdRubro=@idRubro AND IdPresupuesto=@idPresupuesto";


            var parameters = new List<SqlParameter>()
            {
                new SqlParameter(){ ParameterName = "@idRubro", Value = idRubro},
                new SqlParameter(){ ParameterName = "@idPresupuesto", Value = idPresupuesto}
            };

            command.Parameters.Add(parameters[0]);
            command.Parameters.Add(parameters[1]);

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

        public void ActualizaGastoEnPresupuesto(decimal gastoRubro, decimal valorAGastar, string idRubro, int idPresupuesto)
        {
            var conn = _connection.ObtenerConexion();
            
            IDbCommand command = conn.CreateCommand();

            conn.Open();
            command.CommandText = @"UPDATE Presupuesto SET Gastado= @gastado WHERE (IdRubro= @idRubro AND IdPresupuesto= @idPresupuesto)";
            
            var parameters = new List<SqlParameter>()
            {
                new SqlParameter(){ ParameterName = "@gastado", Value = gastoRubro + valorAGastar},
                new SqlParameter(){ ParameterName = "@idRubro", Value = idRubro},
                new SqlParameter(){ ParameterName = "@idPresupuesto", Value = idPresupuesto}
            };
            command.Parameters.Add(parameters[0]);
            command.Parameters.Add(parameters[1]);
            command.Parameters.Add(parameters[2]);

            command.ExecuteNonQuery();
            conn.Close();
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
            var conn = _connection.ObtenerConexion();
            IDbCommand commando = conn.CreateCommand();
            conn.Open();
            commando.CommandText = @"INSERT INTO Gastos(Id,IdPresupuesto,Gasto,Usuario,FechaCreacion, Mes, Anio) VALUES (@idGasto,@idPresupuesto,@gasto,@usuario,@fechaCreacion, @mes, @anio)";


            var parameters = new List<SqlParameter>()
            {
                new SqlParameter(){ ParameterName = "@idGasto", Value = idGasto},
                new SqlParameter(){ ParameterName = "@idPresupuesto", Value = detalle.IdPresupuesto},
                new SqlParameter(){ ParameterName = "@gasto", Value = detalle.Gasto},
                new SqlParameter(){ ParameterName = "@usuario", Value =detalle.Usuario},
                new SqlParameter(){ ParameterName = "@fechaCreacion", Value = DateTime.UtcNow},
                new SqlParameter(){ ParameterName = "@mes", Value = DateTime.UtcNow.AddHours(-3).Month},
                new SqlParameter(){ ParameterName = "@anio", Value = DateTime.UtcNow.AddHours(-3).Year}
            };
        
         
                commando.Parameters.Add(parameters[0]);
                commando.Parameters.Add(parameters[1]);
                commando.Parameters.Add(parameters[2]);
                commando.Parameters.Add(parameters[3]);
                commando.Parameters.Add(parameters[4]);
                commando.Parameters.Add(parameters[5]);
                commando.Parameters.Add(parameters[6]);
 
            commando.ExecuteNonQuery();
            

            conn.Close();

            //Se actualiza gasto en el presupuesto
            this.ActualizaGastoEnPresupuesto(puedeGastarResponse.GastoRubro, detalle.Gasto, detalle.IdRubro, detalle.IdPresupuesto);

            return idGasto;
        }

        public List<Gastos> ObtenerGastos(string command, bool eliminar)
        {
            var conn = _connection.ObtenerConexion();
            IDbCommand commando = conn.CreateCommand();
                  
            commando.CommandText = command;

            conn.Open();
            IDataReader reader = commando.ExecuteReader();

            var gastos = new List<Gastos>();
            if (eliminar)
            {
                reader.Read();
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
            else if (!eliminar)
            {
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
            bool eliminar = true;
            var gastoARestar = this.ObtenerGastos(comando,eliminar)?.FirstOrDefault()?.Gasto;

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

            var parameters = new List<SqlParameter>()
            {
                new SqlParameter(){ ParameterName = "@id", Value = gasto.Id },
                
            };
            command.Parameters.Add(parameters[0]);
          

            command.ExecuteNonQuery();
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
            var conn = _connection.ObtenerConexion();
            IDbCommand command = conn.CreateCommand();
            conn.Open();
            command.CommandText = @"UPDATE Gastos SET Gasto = @gasto WHERE Id = @id AND Usuario = @usuario AND IdPresupuesto = @idPresupuesto; ";

            var parameters = new List<SqlParameter>()
            {
                new SqlParameter(){ ParameterName = "@gasto", Value = detalle.Gasto},
                new SqlParameter(){ ParameterName = "@id", Value = detalle.Id},
                new SqlParameter(){ ParameterName = "@usuario", Value = detalle.Usuario},
                new SqlParameter(){ ParameterName = "@idPresupuesto", Value = detalle.IdPresupuesto},
            };

            command.Parameters.Add(parameters[0]);
            command.Parameters.Add(parameters[1]);
            command.Parameters.Add(parameters[2]);
            command.Parameters.Add(parameters[3]);
      
            command.ExecuteNonQuery();
            conn.Close();
            return "El gasto se actualizó correctamente.";
        }

        private Operacion OperacionEnPresupuesto(EditarGasto detalle)
        {
            // chequea si el gasto editado es mayor o menor al original
            var connection = _connection.ObtenerConexion();

            IDbCommand command = connection.CreateCommand();
            
            decimal gasto = 0;
            command.CommandText = @"SELECT * FROM Gastos where Id=@id;";

            var parameters = new List<SqlParameter>()
            {
                new SqlParameter(){ ParameterName = "@id", Value = detalle.Id},
            };

            command.Parameters.Add(parameters[0]);
        
            connection.Open();
            IDataReader reader = command.ExecuteReader();

            reader.Read();
                    
            //gasto original
            gasto = reader.GetDecimal(2);
                    
                
                //compara los 2 gastos para saber si se debe restar o sumar en la tabla Presupuesto
                if (gasto < detalle.Gasto)
                {
                    connection.Close();
                    decimal valor = detalle.Gasto - gasto;
                    return new Operacion() { op = "+", diferencia = valor };
                }
                if (gasto > detalle.Gasto)
                {
                    connection.Close();
                    decimal valor = gasto - detalle.Gasto;
                    return new Operacion() { op = "-", diferencia = valor };
                }
                else
                {
                    connection.Close();
                    return new Operacion() { op = "=", diferencia = 0 };
                }
            
        }

        private void ActualizaGastoEnPresupuestoPut(decimal valorActual, int idPresupuesto, Operacion operacion)
        {
            var conn = _connection.ObtenerConexion();

            IDbCommand command = conn.CreateCommand();

            conn.Open();

            command.CommandText = @"UPDATE Presupuesto SET Gastado= @gastado WHERE IdPresupuesto = @idPresupuesto;";

            var parameters = new SqlParameter()
            {
                ParameterName = "@idPresupuesto",
                Value = idPresupuesto
            };

            command.Parameters.Add(parameters);

            if (operacion.op == "+")
            {
                var p = new SqlParameter() { ParameterName = "@gastado", Value = valorActual + operacion.diferencia };
                command.Parameters.Add(p);
            }

            if (operacion.op == "-")
            {
                var p = new SqlParameter() { ParameterName = "@gastado", Value = valorActual - operacion.diferencia };
                command.Parameters.Add(p);
            }
            else if (operacion.op == "=")
            {
                var p = new SqlParameter() { ParameterName = "@gastado", Value = valorActual };
                command.Parameters.Add(p);
            }
               command.ExecuteNonQuery();

               conn.Close();
        }

    
    }

    public interface IGastosDB
    {
        Task<List<Gastos>> GastosPorMesAño(string mesAño);
        Task<string> AgregarGasto(AgregarGastoRequest detalle);
        List<Gastos> ObtenerGastos(string command, bool eliminar);
        Task<string> EliminarGasto(EliminaGasto gasto);
        Task<string> ActualizaGasto(EditarGasto detalle);
        PuedeGastarResponse PuedeGastar(string idRubro, decimal valorAGastar, int idPresupuesto);
        void ActualizaGastoEnPresupuesto(decimal gastoRubro, decimal valorAGastar, string idRubro, int idPresupuesto);

    }
}
