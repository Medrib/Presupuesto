using Presupuesto.DataBase;
using System.Data;

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

            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = @"SELECT Usr, Psw FROM Users WHERE Usr = @User";
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@User";
                parameter.Value = user;

                connection.Open();
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return reader.GetString(reader.GetOrdinal("Psw")) == password;
                    }
                    return false;
                }
            }
        }
    }

    public interface IValidacionUsuarioDB
    {
        Task<bool> ValidarUsuario(string user, string password);
    }
}
