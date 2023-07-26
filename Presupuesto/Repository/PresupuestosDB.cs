using Domain.Dtos.Cliente;
using Domain.Shared;
using Presupuesto.DataBase;
using System.Data;
using System.Data.SqlClient;

namespace Presupuesto.Repository
{
    public class PresupuestosDB : IPresupuestosDB
    {
        private readonly IConnection _connection;

        public PresupuestosDB(IConnection connection)
        {
            _connection = connection;
        }
        public async Task<int> AgregarPresupuesto(RequestPresupuesto request)
        {
            Random rnd = new Random();
            var idPresupuesto = rnd.Next(100000, 999999);
            var horarioArg = DateTime.UtcNow.AddHours(-3);

            var cantElementos = request.detallePresupuesto.Count() - 1;
            int i = 0;
            var detalle = new DetallePresupuesto();

            var conn = _connection.ObtenerConexion();
            IDbCommand command = conn.CreateCommand();

            command.Connection = conn;
            command.CommandType = CommandType.Text;
            var parameters = new List<SqlParameter>();
            while (i <= cantElementos)
            {
                command.CommandText = string.Format(@"INSERT INTO Presupuesto(IdPresupuesto,IdRubro,Rubro,Usuario,Presupuesto, Gastado, FechaDeCreacion,Anio,Mes) 
                                            VALUES (@idPresupuesto{0},@idRubro{0},@rubro{0},@usuario{0},@presupuesto{0},@gastado{0}, @fechadecreacion{0},@anio{0}, @mes{0})", i);

                detalle = request.detallePresupuesto[i];
                parameters.Add(new SqlParameter() { ParameterName = "@idPresupuesto" + i, Value = idPresupuesto });
                parameters.Add(new SqlParameter() { ParameterName = "@idRubro" + i, Value = detalle.IdRubro });
                parameters.Add(new SqlParameter() { ParameterName = "@rubro" + i, Value = detalle.Rubro });
                parameters.Add(new SqlParameter() { ParameterName = "@usuario" + i, Value = request.Usuario });
                parameters.Add(new SqlParameter() { ParameterName = "@presupuesto" + i, Value = detalle.Presupuesto });
                parameters.Add(new SqlParameter() { ParameterName = "@gastado" + i, Value = 0 });
                parameters.Add(new SqlParameter() { ParameterName = "@fechadecreacion" + i, Value = horarioArg });
                parameters.Add(new SqlParameter() { ParameterName = "@anio" + i, Value = DateTime.UtcNow.AddHours(-3).Year });
                parameters.Add(new SqlParameter() { ParameterName = "@mes" + i, Value = DateTime.UtcNow.AddHours(-3).Month });
                command.ExecuteNonQuery();

                i++;
            }

            conn.Close();
            return idPresupuesto;
        }

        public async Task<List<EstadoPresupuesto>> SaldoDisponible(string idPresupuesto)
        {
            var conn = _connection.ObtenerConexion();
            IDbCommand command = conn.CreateCommand();

            command.Connection = conn;
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM Presupuesto WHERE Idpresupuesto=@idPresupuesto";


            var parameters = new List<SqlParameter>()
            {
                new SqlParameter(){ ParameterName = "@idPresupuesto", Value = idPresupuesto}
            };

            command.Parameters.Add(parameters);
            conn.Open();

            IDataReader reader = command.ExecuteReader();
            var saldos = new List<EstadoPresupuesto>();
            while (reader.Read())
            {
                var estadoPresupuesto = new EstadoPresupuesto()
                {
                    Rubro = reader.GetString(reader.GetOrdinal("rubro")),
                    Disponible = reader.GetDecimal(reader.GetOrdinal("Presupuesto")) - reader.GetDecimal(reader.GetOrdinal("Gastado"))

                };
                saldos.Add(estadoPresupuesto);
            }

            conn.Close();
            return saldos;
        }


        public async Task<List<PresupuestoModel>> PresupuestoPorFecha(string fecha)
        {
            var fecha2 = Functions.mesAñoIntParse(fecha);

            var connection = _connection.ObtenerConexion();

            IDbCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM Presupuesto WHERE Mes=@mes AND Anio=@anio";

            var parameters = new List<SqlParameter>()
            {
                new SqlParameter(){ ParameterName = "@mes", Value = fecha2.Mes},
                new SqlParameter(){ ParameterName = "@anio", Value = fecha2.Año}
            };

            command.Parameters.Add(parameters);
            connection.Open();

            IDataReader reader = command.ExecuteReader();

            var PresupuestoFecha = new List<PresupuestoModel>();

            while (reader.Read())
            {
                var consulta = new PresupuestoModel()
                {
                    IdPresupuesto = reader.GetInt32(0),
                    IdRubro = reader.GetString(1),
                    Rubro = reader.GetString(2),
                    Usuario = reader.GetString(3),
                    Presupuesto = reader.GetDecimal(4),
                    Gastado = reader.GetDecimal(5),
                    FechaDeCreacion = reader.GetDateTime(6),
                    Mes = reader.GetInt32(7),
                    Anio = reader.GetInt32(8)

                };
                PresupuestoFecha.Add(consulta);
            }
            connection.Close();
            return PresupuestoFecha;
        }
    }

    public interface IPresupuestosDB
    {
        Task<int> AgregarPresupuesto(RequestPresupuesto request);
        Task<List<EstadoPresupuesto>> SaldoDisponible(string idPresupuesto);
        Task<List<PresupuestoModel>> PresupuestoPorFecha(string fecha);
    }
}

