using Azure.Core;
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
            _readerMock.Setup(reader => reader.GetString(0)).Returns("ABC1234567");
            _readerMock.Setup(reader => reader.GetInt32(1)).Returns(123456);
            _readerMock.Setup(reader => reader.GetDecimal(2)).Returns(1000);
            _readerMock.Setup(reader => reader.GetString(3)).Returns("pepito");
            _readerMock.Setup(reader => reader.GetDateTime(4)).Returns(new DateTime(2023, 07, 10, 00, 00, 00));
            _readerMock.Setup(reader => reader.GetInt32(5)).Returns(07);
            _readerMock.Setup(reader => reader.GetInt32(6)).Returns(2023);

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
            //Arrange
            var connectionMock = MockDependencies.GetConnectionMock(_readerMock, _dataParameter);
            _connection.Setup(x => x.ObtenerConexion()).Returns(connectionMock.Object);

            //PuedeGastar
            _readerMock.Setup(reader => reader.GetDecimal(5)).Returns(1000);
            _readerMock.Setup(reader => reader.GetDecimal(4)).Returns(5000);

            //ObtenerGastos
            _readerMock.Setup(reader => reader.GetString(0)).Returns("ABC0123456");
            _readerMock.Setup(reader => reader.GetInt32(1)).Returns(123456);
            _readerMock.Setup(reader => reader.GetDecimal(2)).Returns(1000);
            _readerMock.Setup(reader => reader.GetString(3)).Returns("pepito");
            _readerMock.Setup(reader => reader.GetDateTime(4)).Returns(new DateTime(2023, 07, 10, 00, 00, 00));
            _readerMock.Setup(reader => reader.GetInt32(5)).Returns(7);
            _readerMock.Setup(reader => reader.GetInt32(6)).Returns(2023);

            var gasto = new EliminaGasto()
            {
                Id = "ABC1234567",
                IdPresupuesto = 123456,
                IdRubro = "ABC"
            };

            //
            string message = "El gasto se eliminó correctamente.";
            //Act
            var res = await _gastosDB.EliminarGasto(gasto);

            //Assert
            Assert.Equal(message, res);
        }

        [Fact]
        public async Task AgregarGasto_ok()
        {
            // arrange
            var request = new AgregarGastoRequest()
            {
                Gasto = 443,
                IdPresupuesto = 1000,
                IdRubro = "",
                Usuario = "cristian"

            };
            _readerMock.Setup(reader => reader.GetDecimal(5)).Returns(100);
            _readerMock.Setup(reader => reader.GetDecimal(4)).Returns(3000);
            _dataParameter.Setup(command => command.Add(_parametersMock.Object));

            var connectionMock = MockDependencies.GetConnectionMock(_readerMock, _dataParameter);
            _connection.Setup(x => x.ObtenerConexion())
                .Returns(connectionMock.Object);

            //Act
            var res = await _gastosDB.AgregarGasto(request);

            // assert
            Assert.NotNull(res);
        }

        [Fact]
        public async Task ActualizaGasto_ok()
        {
            //Arrage
            var connectionMock = MockDependencies.GetConnectionMock(_readerMock, _dataParameter);
            _connection.Setup(x => x.ObtenerConexion()).Returns(connectionMock.Object);

            _readerMock.Setup(reader => reader.GetDecimal(5)).Returns(500);
            _readerMock.Setup(reader => reader.GetDecimal(4)).Returns(5000);
            _readerMock.Setup(reader => reader.GetDecimal(2)).Returns(1000);
       
            var editarGasto = new EditarGasto()
            {
                Id = "IND00000001",
                IdPresupuesto = 123456,
                IdRubro = "IND",
                Gasto = 1500,
                Usuario = "usuario13"
            };
            var message = "El gasto se actualizó correctamente.";

            //Act
            var res = await _gastosDB.ActualizaGasto(editarGasto);

            //Assert
            Assert.Equal(message, res);
        }
    }
}
