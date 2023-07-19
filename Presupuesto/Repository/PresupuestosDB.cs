using Domain.Dtos.Cliente;
using System.Data.SqlClient;
using Presupuesto.DataBase;
using System.Data;



namespace Presupuesto.Repository
{
    public class PresupuestosDB
    {
        public async Task<int> AgregarPresupuesto(RequestPresupuesto request)
        {
            Random rnd = new Random();
            var idPresupuesto = rnd.Next(100000, 999999);
            //var idPresupuesto = request.IdRubro + parteEntera;

            var horarioArg = DateTime.UtcNow.AddHours(-3);

            var cantElementos = request.detallePresupuesto.Count() - 1;
            int i = 0;
            var detalle = new DetallePresupuesto();
            using (SqlConnection conn = Connection.ObtenerConexion())
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;

                    while (i <= cantElementos)
                    {
                        cmd.CommandText = string.Format(@"INSERT INTO Presupuesto(IdPresupuesto,IdRubro,Rubro,Responsable,Estimado, GastoRubro, FechaInicio,FechaFin) 
                                            VALUES (@idPresupuesto{0},@idRubro{0},@rubro{0},@responsable{0},@estimado{0},@gastoRubro{0}, @fechaInicio{0},@fechaFin{0})", i);

                        detalle = request.detallePresupuesto[i];
                        cmd.Parameters.AddWithValue("@idPresupuesto" + i, idPresupuesto);
                        cmd.Parameters.AddWithValue("@idRubro" + i, detalle.IdRubro);
                        cmd.Parameters.AddWithValue("@rubro" + i, detalle.Rubro);
                        cmd.Parameters.AddWithValue("@responsable" + i, request.Responsable);
                        cmd.Parameters.AddWithValue("@estimado" + i, detalle.Estimado);
                        cmd.Parameters.AddWithValue("@gastoRubro" + i, 0);
                        cmd.Parameters.AddWithValue("@fechaInicio" + i, horarioArg);
                        cmd.Parameters.AddWithValue("@fechaFin" + i, horarioArg.AddDays(request.DuracionPresupuesto));
                        cmd.ExecuteNonQuery();

                        i++;
                    }

                    //conn.Open();

                    conn.Close();
                    return idPresupuesto;
                }
            }
        }

        public async Task<List<EstadoPresupuesto>> EstadoPresupuesto(string idPresupuesto)
        {
            using (SqlConnection connection = Connection.ObtenerConexion())
            {
                SqlCommand command = new SqlCommand(
                    string.Format("SELECT IdPresupuesto, IdRubro, Rubro, Responsable, Estimado, GastoRubro, FechaInicio, FechaFin FROM Presupuesto WHERE Idpresupuesto={0}", idPresupuesto),
                      connection

                );

                SqlDataReader reader = command.ExecuteReader();

                var consulta = new List<EstadoPresupuesto>();
         
                    while (reader.Read())
                    {
                      var presupuesto = new EstadoPresupuesto()
                       {
                           Rubro = reader.GetString("Rubro"),
                           Disponible = reader.GetDecimal("Estimado") - reader.GetDecimal("GastoRubro")

                       };
                       consulta.Add(presupuesto);
                    }
           
                connection.Close();
                return consulta;
            }
        }
    }
}

