using Domain.Dtos.Cliente;
using Moq;
using Presupuesto.DataBase;
using Presupuesto.Repository;
using System.Data;

namespace TestPresupuesto
{
    public class GastosDBUnitTest
    {
        private readonly Mock<IConnection> _connection;
        private readonly Mock<IDataReader> _readerMock;
        private readonly Mock<IDbDataParameter> _parametersMock;
        private readonly Mock<IDataParameterCollection> _dataParameter;
        private readonly GastosDB _gastosDB;

        public GastosDBUnitTest()
        {
            _connection = new Mock<IConnection>();
            _readerMock = new Mock<IDataReader>();
            _parametersMock = new Mock<IDbDataParameter>();
            _dataParameter = new Mock<IDataParameterCollection>();
            _gastosDB = new GastosDB(_connection.Object);
        }
        [Fact]
        public async Task GastosPorMesAño_ok()
        {
            //Arrange
            _readerMock.Setup(reader => reader.GetOrdinal("Id")).Returns(0);
            _readerMock.Setup(reader => reader.GetString(0)).Returns("ABC1234567");

            _readerMock.Setup(reader => reader.GetOrdinal("IdPresupuesto")).Returns(1);
            _readerMock.Setup(reader => reader.GetInt32(1)).Returns(123456);

            _readerMock.Setup(reader => reader.GetOrdinal("Gasto")).Returns(2);
            _readerMock.Setup(reader => reader.GetDecimal(2)).Returns(1000);

            _readerMock.Setup(reader => reader.GetOrdinal("Usuario")).Returns(3);
            _readerMock.Setup(reader => reader.GetString(3)).Returns("pepito");

            _readerMock.Setup(reader => reader.GetOrdinal("FechaCreacion")).Returns(4);
            _readerMock.Setup(reader => reader.GetDateTime(4)).Returns(new DateTime(2023, 07, 10, 00,00,00));

            _readerMock.Setup(reader => reader.GetOrdinal("Mes")).Returns(5);
            _readerMock.Setup(reader => reader.GetInt32(5)).Returns(07);

            _readerMock.Setup(reader => reader.GetOrdinal("Anio")).Returns(6);
            _readerMock.Setup(reader => reader.GetInt32(6)).Returns(2023);

            //
            _parametersMock.Setup(p => p.ParameterName).Returns("@mes");
            _parametersMock.Setup(v => v.Value).Returns(07);

            _parametersMock.Setup(p => p.ParameterName).Returns("@anio");
            _parametersMock.Setup(v => v.Value).Returns(2023);

            _dataParameter.Setup(command => command.Add(_parametersMock.Object));

            //
            var fecha = "2023-07";
            var response = new List<Gastos>()
            {
                new Gastos()
                {
                    Id = "ABC1234567",
                    IdPresupuesto = 123456,
                    Gasto = 1000,
                    Usuario ="pepito",
                    FechaCreacion = new DateTime(2023, 07, 10, 00,00,00),
                    Mes = 07,
                    Año = 2023,
                }
            };

            //
            var connectionMock = MockDependencies.GetConnectionMock(_readerMock, _dataParameter);
            _connection.Setup(x => x.ObtenerConexion()).Returns(connectionMock.Object);

            //Act
            var res = await _gastosDB.GastosPorMesAño(fecha);

            //Assert
            Assert.Equal(response[0].Id, res[0].Id);
         
        }
        [Fact]
        public async Task EliminarGasto_ok()
        {
            //falta mock en los metodos.
            //Arrange
            var connectionMock = MockDependencies.GetConnectionMock(_readerMock, _dataParameter);
            _connection.Setup(x => x.ObtenerConexion()).Returns(connectionMock.Object);

            var interfaceGastoDB = new Mock<IGastosDB>();
         
            interfaceGastoDB.Setup(x => x.PuedeGastar(
                It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<int>()))
                .Returns(new PuedeGastarResponse() { GastoRubro = 1000, PuedeGastar = true }
                );
            
            //interfaceGastoDB.Setup(x => x.ActualizaGastoEnPresupuesto(
            //    It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<int>()));

            //var listaGasto = new List<Gastos>()
            //{
            //    new Gastos()
            //    {
            //        Id = "ABC1234567",
            //        IdPresupuesto = 123456,
            //        Gasto = 2500,
            //        Usuario ="pepito",
            //        FechaCreacion = new DateTime(2023, 07, 10, 00,00,00),
            //        Mes = 07,
            //        Año = 2023,
            //    }
            //};
            //interfaceGastoDB.Setup(x => x.ObtenerGastos(
            //    It.IsAny<string>())).Returns(listaGasto);

            //
            var gasto = new EliminaGasto()
            {
                Id = "ABC1234567",
                IdPresupuesto = 123456,
                IdRubro = "ABC"
            };

            //
            _parametersMock.Setup(p => p.ParameterName).Returns("@id");
            _parametersMock.Setup(v => v.Value).Returns("ABC1234567");

            _dataParameter.Setup(command => command.Add(_parametersMock.Object));

            string message = "El gasto se eliminó correctamente.";
            //Act
            var res = await _gastosDB.EliminarGasto(gasto);

            //Assert
            Assert.Equal(message, res);
        }

    }
}
