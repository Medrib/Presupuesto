using Presupuesto.DataBase;
using System.Data;
using System.Data.SqlClient;

namespace Presupuesto.Repository
{
    public class ValidacionUsuarioDB : IValidacionUsuarioDB
    {
        private readonly IConnection _connection;
        public ValidacionUsuarioDB(IConnection connection)
        {
            _connection = connection;
        }
        public async Task<bool> ValidarUsuario(string user, string password)
        {
            var connection = _connection.ObtenerConexion();

            IDbCommand command = connection.CreateCommand();

            command.CommandText = @"SELECT Usr, Psw FROM Users WHERE Usr = @User";
            var parameters = new List<SqlParameter>()
            {
                new SqlParameter()
                {
                    ParameterName = "@User",
                    Value = user,
                }
            };

            command.Parameters.Add(parameters);
            connection.Open();

            IDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                return reader.GetString(reader.GetOrdinal("Psw")) == password;
            }
            return false;
        }
    }

    public interface IValidacionUsuarioDB
    {
        Task<bool> ValidarUsuario(string user, string password);
    }
}
