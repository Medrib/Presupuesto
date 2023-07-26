using Domain.Dtos.Cliente;
using Moq;
using Presupuesto.DataBase;
using Presupuesto.Repository;
using System.Data;
using System.Reflection.PortableExecutable;

namespace TestPresupuesto
{
    public class GastosDBUnitTest
    {
        private readonly Mock<IConnection> _connection;
        private readonly GastosDB _gastosDB;

        public GastosDBUnitTest()
        {
            _connection = new Mock<IConnection>();
            _gastosDB = new GastosDB(_connection.Object);
        }

        [Fact]
        public async Task GastosPorMesAño_ok()
        {
            //Arrange
            var readerMock = new Mock<IDataReader>();
            readerMock.Setup(reader => reader.GetOrdinal("Id")).Returns(0);
            readerMock.Setup(reader => reader.GetString(0)).Returns("ABC1234567");

            readerMock.Setup(reader => reader.GetOrdinal("IdPresupuesto")).Returns(1);
            readerMock.Setup(reader => reader.GetInt32(1)).Returns(123456);

            readerMock.Setup(reader => reader.GetOrdinal("Gasto")).Returns(2);
            readerMock.Setup(reader => reader.GetDecimal(2)).Returns(1000);

            readerMock.Setup(reader => reader.GetOrdinal("Usuario")).Returns(3);
            readerMock.Setup(reader => reader.GetString(3)).Returns("pepito");

            readerMock.Setup(reader => reader.GetOrdinal("FechaCreacion")).Returns(4);
            readerMock.Setup(reader => reader.GetDateTime(4)).Returns(new DateTime(2023, 07, 10, 00,00,00));

            readerMock.Setup(reader => reader.GetOrdinal("Mes")).Returns(5);
            readerMock.Setup(reader => reader.GetInt32(5)).Returns(07);

            readerMock.Setup(reader => reader.GetOrdinal("Anio")).Returns(6);
            readerMock.Setup(reader => reader.GetInt32(6)).Returns(2023);

            //
            var parametersMock = new Mock<IDbDataParameter>();
            parametersMock.Setup(p => p.ParameterName).Returns("@mes");
            parametersMock.Setup(v => v.Value).Returns(07);

            parametersMock.Setup(p => p.ParameterName).Returns("@anio");
            parametersMock.Setup(v => v.Value).Returns(2023);

            var parameters = new Mock<IDataParameterCollection>();
            parameters.Setup(command => command.Add(parametersMock.Object));

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
            var connectionMock = MockDependencies.GetConnectionMock(readerMock, parameters);
            _connection.Setup(x => x.ObtenerConexion()).Returns(connectionMock.Object);

            //Act
            var res = await _gastosDB.GastosPorMesAño(fecha);

            //Assert
            Assert.Equal(response[0].Id, res[0].Id);
         
        }

        [Fact]
        public async Task EliminarGasto_ok()
        {
            //

            var readerMock = new Mock<IDataReader>();
            var parameters = new Mock<IDataParameterCollection>();

            var connectionMock = MockDependencies.GetConnectionMock(readerMock, parameters);
            _connection.Setup(x => x.ObtenerConexion()).Returns(connectionMock.Object);

            var interfaceGastosDB = new Mock<IGastosDB>();

            interfaceGastosDB.Setup(x => x.PuedeGastar(It.IsAny<String>(), It.IsAny<Decimal>(), It.IsAny<int>()))
                .Returns(new PuedeGastarResponse() { GastoRubro = 100 });

            var gasto = new EliminaGasto()
            {
                IdRubro = "IND",
                IdPresupuesto = 123456
            };

            //Act
            var res = await _gastosDB.EliminarGasto(gasto);

            //Assert
            //Assert.Equal(response[0].Id, res[0].Id);
        }
    }
}
