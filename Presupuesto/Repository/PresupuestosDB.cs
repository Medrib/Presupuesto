using Domain.Dtos.Cliente;
using Presupuesto.DataBase;

namespace Presupuesto.Repository
{
    public class PresupuestosDB
    {
        private readonly IConnection _connection;

        public PresupuestosDB(IConnection connection)
        {
            _connection = connection;
        }
        public async Task<int> AgregarPresupuesto(RequestPresupuesto request)
        {
            //Random rnd = new Random();
            //var idPresupuesto = rnd.Next(100000, 999999);
            ////var idPresupuesto = request.IdRubro + parteEntera;

            //var horarioArg = DateTime.UtcNow.AddHours(-3);

            //var cantElementos = request.detallePresupuesto.Count() - 1;
            //int i = 0;
            //var detalle = new DetallePresupuesto();
            //using (SqlConnection conn = _connection.ObtenerConexion())
            //{
            //    using (SqlCommand cmd = new SqlCommand())
            //    {
            //        cmd.Connection = conn;
            //        cmd.CommandType = CommandType.Text;

            //        while (i <= cantElementos)
            //        {
            //            cmd.CommandText = string.Format(@"INSERT INTO Presupuesto(IdPresupuesto,IdRubro,Rubro,Usuario,Presupuesto, Gastado, FechaDeCreacion,Anio,Mes) 
            //                                VALUES (@idPresupuesto{0},@idRubro{0},@rubro{0},@usuario{0},@presupuesto{0},@gastado{0}, @fechadecreacion{0},@anio{0}, @mes{0})", i);

            //            detalle = request.detallePresupuesto[i];
            //            cmd.Parameters.AddWithValue("@idPresupuesto" + i, idPresupuesto);
            //            cmd.Parameters.AddWithValue("@idRubro" + i, detalle.IdRubro);
            //            cmd.Parameters.AddWithValue("@rubro" + i, detalle.Rubro);
            //            cmd.Parameters.AddWithValue("@usuario" + i, request.Usuario);
            //            cmd.Parameters.AddWithValue("@presupuesto" + i, detalle.Presupuesto);
            //            cmd.Parameters.AddWithValue("@gastado" + i, 0);
            //            cmd.Parameters.AddWithValue("@fechadecreacion" + i, horarioArg);
            //            cmd.Parameters.AddWithValue("@anio" + i, DateTime.UtcNow.AddHours(-3).Year);
            //            cmd.Parameters.AddWithValue("@mes" + i, DateTime.UtcNow.AddHours(-3).Month);
            //            cmd.ExecuteNonQuery();

            //            i++;
            //        }

            //        //conn.Open();

            //        conn.Close();
            //        return idPresupuesto;
            return 0;
            //    }
            //}
        }

        public async Task<List<EstadoPresupuesto>> SaldoDisponible(string idPresupuesto)
        {
        //    SqlConnection conn = _connection.ObtenerConexion();
        //    SqlCommand cmd = new SqlCommand();
        //    cmd.Connection = conn;
        //    cmd.CommandType = CommandType.Text;
        //    cmd.CommandText = "SELECT * FROM Presupuesto WHERE Idpresupuesto=@idPresupuesto";

        //    cmd.Parameters.AddWithValue("@idPresupuesto", idPresupuesto);

        //    SqlDataReader reader = cmd.ExecuteReader();

        //    var saldos = new List<EstadoPresupuesto>();

        //    while (reader.Read())
        //    {
        //        var estadoPresupuesto = new EstadoPresupuesto()
        //        {
        //            Rubro = reader.GetString("Rubro"),
        //            Disponible = reader.GetDecimal("Presupuesto") - reader.GetDecimal("Gastado")

        //        };
        //        saldos.Add(estadoPresupuesto);
        //    }

        //    conn.Close();
        //    return saldos;
            return new List<EstadoPresupuesto>();
        }


        public async Task<List<PresupuestoModel>> PresupuestoPorFecha(string fecha)
        {
            //var fecha2 = Functions.mesAñoIntParse(fecha);

            //using (SqlConnection connection = _connection.ObtenerConexion())
            //{
            //    SqlCommand command = new SqlCommand(
            //        string.Format("SELECT * FROM Presupuesto WHERE Mes={0} AND Anio={1};", fecha2.Mes, fecha2.Año),
            //          connection
            //    );

            //    SqlDataReader reader = command.ExecuteReader();

            //    var PresupuestoFecha = new List<PresupuestoModel>();

            //    while (reader.Read())
            //    {
            //        var consulta = new PresupuestoModel()
            //        {
            //            IdPresupuesto = reader.GetInt32(0),
            //            IdRubro = reader.GetString(1),
            //            Rubro = reader.GetString(2),
            //            Usuario = reader.GetString(3),
            //            Presupuesto = reader.GetDecimal(4),
            //            Gastado = reader.GetDecimal(5),
            //            FechaDeCreacion = reader.GetDateTime(6),
            //            Mes = reader.GetInt32(7),
            //            Anio = reader.GetInt32(8)

            //        };
            //        PresupuestoFecha.Add(consulta);
            //    }
            //    connection.Close();
            //    return PresupuestoFecha;
            //}

            return new List<PresupuestoModel>();
        }
    }
}

