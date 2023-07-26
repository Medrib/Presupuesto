using Moq;
using Presupuesto.DataBase;
using Presupuesto.Repository;
using System.Data;

namespace TestPresupuesto
{
    public class ValidacionUsuarioDBUnitTest
    {
        private readonly Mock<IConnection> _connection;
        private readonly ValidacionUsuarioDB _validacionUsuarioDB;
        public ValidacionUsuarioDBUnitTest()
        {
            _connection = new Mock<IConnection>();
            _validacionUsuarioDB = new ValidacionUsuarioDB(_connection.Object);
        }


        [Fact]
        public async Task ValidarUsuario_Ok()
        {
            //Arrange

            var usr = "pepito";
            var pwd = "1234";

            //
            var readerMock = new Mock<IDataReader>();
            readerMock.Setup(reader => reader.GetOrdinal("Psw")).Returns(0);
            readerMock.Setup(reader => reader.GetString(0)).Returns(pwd);

            //
            var parameters = new Mock<IDataParameterCollection>();

            var parameterMock = new Mock<IDbDataParameter>();
            parameterMock.Setup(_ => _.ParameterName).Returns("@User");
            parameterMock.Setup(_ => _.Value).Returns(usr);

            parameters.Setup(parameters => parameters.Add(parameterMock.Object));

            //
            var connectionMock = MockDependencies.GetConnectionMock(readerMock, parameters);
            _connection.Setup(x => x.ObtenerConexion())
                .Returns(connectionMock.Object);

            //Act
            var response = await _validacionUsuarioDB.ValidarUsuario(usr, pwd);

            //Assert
            Assert.True(response);
        }
    }
}
