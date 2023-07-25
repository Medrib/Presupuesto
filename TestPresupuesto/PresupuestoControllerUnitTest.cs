using Moq;
using Presupuesto.Controllers;
using Presupuesto.DataBase;
using Presupuesto.Repository;
using System.Data;

namespace TestPresupuesto
{
    public class PresupuestoControllerUnitTest
    {
        private readonly PresupuestoController _presupuestoController;
        private readonly Mock<GastosDB> _gastosDB;
        private readonly Mock<PresupuestosDB> _presupuestosDB;
        private readonly Mock<ValidacionUsuarioDB> _validacionUsuarioDB;
        private readonly Mock<IConnection> _connection;
        public PresupuestoControllerUnitTest()
        {
            //ACA INICIALIZO LAS COSAS QUE VOY A NECESITAR EN LOS TEST DE ESTE ARCHIVO - GENERICAMENTE
            _connection = new Mock<IConnection>();
            _gastosDB = new Mock<GastosDB>(_connection.Object);
            _presupuestosDB = new Mock<PresupuestosDB>(_connection.Object);
            _validacionUsuarioDB = new Mock<ValidacionUsuarioDB>(_connection.Object);
            _presupuestoController =  new PresupuestoController(_gastosDB.Object, _presupuestosDB.Object, _validacionUsuarioDB.Object);
        }
        [Fact]
        public async Task ValidarUsuario_Ok()
        {
            //Arrange
            var usr = "cristian";
            var pwd = "1234";

            var readerMock = new Mock<IDataReader>();

            readerMock.SetupSequence(_ => _.Read())
                .Returns(true)
                .Returns(false);

            readerMock.Setup(reader => reader.GetOrdinal("Psw")).Returns(0);
            readerMock.Setup(reader => reader.GetString(0)).Returns(pwd);
            var commandMock = new Mock<IDbCommand>();
            commandMock.Setup(m => m.ExecuteReader()).Returns(readerMock.Object).Verifiable();

            var parameterMock = new Mock<IDbDataParameter>();
            parameterMock.Setup(_ => _.ParameterName).Returns("@User");
            parameterMock.Setup(_ => _.Value).Returns(usr);
            commandMock.Setup(m => m.CreateParameter()).Returns(parameterMock.Object).Verifiable();

            var connectionMock = new Mock<IDbConnection>();
            connectionMock.Setup(m => m.CreateCommand()).Returns(commandMock.Object);
            _connection.Setup(x => x.ObtenerConexion())
                .Returns(connectionMock.Object);

            //Act
            var response = await _presupuestoController.ValidarUsuario(usr, pwd);

            //Assert
            Assert.True(response);
        }
    }
}