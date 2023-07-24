using Moq;
using Presupuesto.Controllers;
using Presupuesto.DataBase;
using Presupuesto.Repository;
using System.Data.SqlClient;

namespace TestPresupuesto
{
    public class PresupuestoControllerUnitTest
    {
        private readonly PresupuestoController _presupuestoController;
        private readonly Mock<GastosDB> _gastosDB;
        private readonly Mock<PresupuestosDB> _presupuestosDB;
        private readonly Mock<ValidacionUsuarioDB> _validacionUsuarioDB;
        public PresupuestoControllerUnitTest()
        {
            //ACA INICIALIZO LAS COSAS QUE VOY A NECESITAR EN LOS TEST DE ESTE ARCHIVO - GENERICAMENTE
            _gastosDB = new Mock<GastosDB>(new Connection());
            _presupuestosDB = new Mock<PresupuestosDB>(new Connection());
            _validacionUsuarioDB = new Mock<ValidacionUsuarioDB>(new Connection());
            _presupuestoController =  new PresupuestoController(_gastosDB.Object, _presupuestosDB.Object, _validacionUsuarioDB.Object);
            //_connection = new Mock<Connection>();
        }
        [Fact]
        public async Task ValidarUsuario_Ok()
        {
            //Arrange -- ACA DECLARO Y ASIGNO LO QUE VOY A NECESITAR PARA TESTEAR ESPECIFICAMENTE PARA ESTE TEST
            var usr = "cristian";
            var pwd = "1234";

            var _connection = new Mock<IConnection>();

            _connection.Setup(x => x.ObtenerConexion())
                .Returns(new SqlConnection());

            //var sqlConnection = new SqlConnection("");
            //sqlConnection.Open();


            //var readerMock = new Mock<SqlDataReader>();

            //readerMock.SetupSequence(_ => _.Read())
            //    .Returns(true)
            //    .Returns(false);

            //readerMock.Setup(reader => reader.GetString(It.IsAny<String>())).Returns(pwd);


            //readerMock.Setup(reader => reader.GetInt32(It.IsAny<int>())).Returns(1);
            //readerMock.Setup(reader => reader.GetString(It.IsAny<int>())).Returns("Hello World");

            //var commandMock = new Mock<SqlCommand>();
            //commandMock.Setup(m => m.ExecuteReader()).Returns(readerMock.Object).Verifiable();

            //var connectionMock = new Mock<IDbConnection>();
            //connectionMock.Setup(m => m.CreateCommand()).Returns(commandMock.Object);

            //var data = new Data(() => connectionMock.Object);


            //Act -- ACA LLAMO AL SERVICIO 
            var response = await _presupuestoController.ValidarUsuario(usr, pwd);

            //Assert -- ACA PONGO LOS ASSERT Y CONDICIONO LOS RESULTADOS.
            Assert.NotNull(response);
        }
    }
}