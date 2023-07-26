using Moq;
using Presupuesto.Controllers;
using Presupuesto.DataBase;
using Presupuesto.Repository;
using System.Data;

namespace TestPresupuesto
{
    public class PresupuestoControllerUnitTest
    {
        private readonly Mock<IConnection> _connection;
        private readonly PresupuestoController _presupuestoController;
        private readonly Mock<IGastosDB> _gastosDB;
        private readonly Mock<IPresupuestosDB> _presupuestosDB;
        private readonly Mock<IValidacionUsuarioDB> _validacionUsuarioDB;
        public PresupuestoControllerUnitTest()
        {
            //ACA INICIALIZO LAS COSAS QUE VOY A NECESITAR EN LOS TEST DE ESTE ARCHIVO - GENERICAMENTE
            _connection = new Mock<IConnection>();
            _gastosDB = new Mock<IGastosDB>();
            _presupuestosDB = new Mock<IPresupuestosDB>();
            _validacionUsuarioDB = new Mock<IValidacionUsuarioDB>();
            _presupuestoController =  new PresupuestoController(_validacionUsuarioDB.Object, _gastosDB.Object, _presupuestosDB.Object);
        }
        [Fact]
        public async Task ValidarUsuario_Ok()
        {
            //Arrange -- ACA DECLARO Y ASIGNO LO QUE VOY A NECESITAR PARA TESTEAR ESPECIFICAMENTE PARA ESTE TEST
            var usr = "pepito";
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
        //[Fact]
        //public async Task AgregarPresupuesto_ok()
        //{
        //    //Arrange
        //    RequestPresupuesto nuevoPresupuesto = new RequestPresupuesto();

        //    _presupuestosDB.Setup(r => r.AgregarPresupuesto(nuevoPresupuesto)).ReturnsAsync(100000);

        //    //RequestPresupuesto nuevoPresupuesto = new RequestPresupuesto()
        //    //{
        //    //    Usuario = "pepito",
        //    //    DuracionPresupuesto = 30,
        //    //    detallePresupuesto = new List<DetallePresupuesto> { detalle }
        //    //};

        //    //Act
        //    var response = _presupuestoController;

        //    //Assert
        //    Assert.NotNull(response);
        //}

        //[Fact]
        //public async Task SaldoDisponible_ok()
        //{

        //}
    }
}