﻿
using Domain.Dtos.Cliente;
using Moq;
using Presupuesto.DataBase;
using Presupuesto.Repository;
using System.Collections.Generic;
using System.Data;

namespace TestPresupuesto
{
    public class PresupuestosDBUnitTest
    {
        private readonly Mock<IConnection> _connection;
        private readonly PresupuestosDB _presupuestosDB;
        public PresupuestosDBUnitTest()
        {
            _connection = new Mock<IConnection>();
            _presupuestosDB = new PresupuestosDB(_connection.Object);
        }
        [Fact]
        public async Task AgregarPresupuesto_OK()
        {

            var request = new RequestPresupuesto()
            {

                Usuario = "cristian",
                DuracionPresupuesto = 30,

                detallePresupuesto = new List<DetallePresupuesto>()
                {
                    new DetallePresupuesto
                    {
                        IdRubro = "IND",
                        Presupuesto =30000,
                        Rubro = "INDUMENTARIA"
                        

                    }
                }
            };
            var readerMock = new Mock<IDataReader>();
            readerMock.Setup(reader => reader.GetOrdinal("@idPresupuesto")).Returns(0);
            readerMock.Setup(reader => reader.GetInt32(0)).Returns(123456);

            readerMock.Setup(reader => reader.GetOrdinal("@idRubro")).Returns(1);
            readerMock.Setup(reader => reader.GetString(1)).Returns("IND");

            readerMock.Setup(reader => reader.GetOrdinal("@rubro")).Returns(2);
            readerMock.Setup(reader => reader.GetString(2)).Returns("INDUMENTARIA");

            readerMock.Setup(reader => reader.GetOrdinal("@usuario")).Returns(3);
            readerMock.Setup(reader => reader.GetString(3)).Returns("cristian");

            readerMock.Setup(reader => reader.GetOrdinal("@presupuesto")).Returns(4);
            readerMock.Setup(reader => reader.GetDecimal(4)).Returns(15000);

            readerMock.Setup(reader => reader.GetOrdinal("@gastado")).Returns(5);
            readerMock.Setup(reader => reader.GetDecimal(5)).Returns(3000);

            readerMock.Setup(reader => reader.GetOrdinal("@fechaDeCreacion")).Returns(6);
            readerMock.Setup(reader => reader.GetDateTime(6)).Returns(new DateTime(2016, 6, 1, 6, 34, 53));

            readerMock.Setup(reader => reader.GetOrdinal("@mes")).Returns(7);
            readerMock.Setup(reader => reader.GetInt32(7)).Returns(05);

            readerMock.Setup(reader => reader.GetOrdinal("@anio")).Returns(8);
            readerMock.Setup(reader => reader.GetInt32(8)).Returns(2023);
            //

            var parameters = new Mock<IDataParameterCollection>();

            var connectionMock = MockDependencies.GetConnectionMock(readerMock, parameters);
            _connection.Setup(x => x.ObtenerConexion())
                .Returns(connectionMock.Object);

            //ACT
           var res = await _presupuestosDB.AgregarPresupuesto(request);

            // Assert
            Assert.NotNull(res);    

        }




        [Fact]
        public async Task PresupuestoPorFecha_Ok()
        {
            //Arrange

            var fecha = "2023-05";

            var response = new List<PresupuestoModel>()
            {
                new PresupuestoModel()
                {
                    IdPresupuesto = 123456,
                    IdRubro = "TECN",
                    Rubro = "TECNOLOGIA",
                    Usuario = "pepito",
                    Presupuesto = 15000,
                    Gastado = 3000,
                    FechaDeCreacion = new DateTime(2016, 6, 1, 6, 34, 53),
                    Mes = 05,
                    Anio = 2023
                }
            };
            //
            var readerMock = new Mock<IDataReader>();
            readerMock.Setup(reader => reader.GetOrdinal("IdPresupuesto")).Returns(0);
            readerMock.Setup(reader => reader.GetInt32(0)).Returns(123456);

            readerMock.Setup(reader => reader.GetOrdinal("IdRubro")).Returns(1);
            readerMock.Setup(reader => reader.GetString(1)).Returns("TECN");

            readerMock.Setup(reader => reader.GetOrdinal("Rubro")).Returns(2);
            readerMock.Setup(reader => reader.GetString(2)).Returns("TECNOLOGIA");

            readerMock.Setup(reader => reader.GetOrdinal("Usuario")).Returns(3);
            readerMock.Setup(reader => reader.GetString(3)).Returns("pepito");

            readerMock.Setup(reader => reader.GetOrdinal("Presupuesto")).Returns(4);
            readerMock.Setup(reader => reader.GetDecimal(4)).Returns(15000);

            readerMock.Setup(reader => reader.GetOrdinal("Gastado")).Returns(5);
            readerMock.Setup(reader => reader.GetDecimal(5)).Returns(3000);

            readerMock.Setup(reader => reader.GetOrdinal("FechaDeCreacion")).Returns(6);
            readerMock.Setup(reader => reader.GetDateTime(6)).Returns(new DateTime(2016, 6, 1, 6, 34, 53));

            readerMock.Setup(reader => reader.GetOrdinal("Mes")).Returns(7);
            readerMock.Setup(reader => reader.GetInt32(7)).Returns(05);

            readerMock.Setup(reader => reader.GetOrdinal("Anio")).Returns(8);
            readerMock.Setup(reader => reader.GetInt32(8)).Returns(2023);

            //
            var parameters = new Mock<IDataParameterCollection>();

            var parameterMock = new Mock<IDbDataParameter>();
            parameterMock.Setup(_ => _.ParameterName).Returns("@mes");
            parameterMock.Setup(_ => _.Value).Returns(05);


            parameterMock.Setup(_ => _.ParameterName).Returns("@anio");
            parameterMock.Setup(_ => _.Value).Returns(2023);

            parameters.Setup(parameters => parameters.Add(parameterMock.Object));

            //
            var connectionMock = MockDependencies.GetConnectionMock(readerMock, parameters);
            _connection.Setup(x => x.ObtenerConexion())
                .Returns(connectionMock.Object);

            //Act
            var res = await _presupuestosDB.PresupuestoPorFecha(fecha);

            //Assert
            Assert.Equal(response[0].Rubro, res[0].Rubro);
        }
        [Fact]
        public async Task SaldoDisponible_OK()
        {
            // Arrange 
            var estadoPresupuesto = new EstadoPresupuesto()
            {
                Rubro = "INDUMENTARIA",
                Disponible = 2000
            };

            //
            var readerMock = new Mock<IDataReader>();
            readerMock.Setup(reader => reader.GetOrdinal("rubro")).Returns(0);
            readerMock.Setup(reader => reader.GetString(0)).Returns("INDUMENTARIA");

            readerMock.Setup(reader => reader.GetOrdinal("Presupuesto")).Returns(1);
            readerMock.Setup(reader => reader.GetDecimal(1)).Returns(3000);

            readerMock.Setup(reader => reader.GetOrdinal("Gastado")).Returns(2);
            readerMock.Setup(reader => reader.GetDecimal(2)).Returns(1000);

            //
            var parameters = new Mock<IDataParameterCollection>();

            var parameterMock = new Mock<IDbDataParameter>();
            parameterMock.Setup(_ => _.ParameterName).Returns("@idPresupuesto");
            parameterMock.Setup(_ => _.Value).Returns("123456");

            parameters.Setup(parameters => parameters.Add(parameterMock.Object));
            //
            var connectionMock = MockDependencies.GetConnectionMock(readerMock, parameters);
            _connection.Setup(x => x.ObtenerConexion())
                .Returns(connectionMock.Object);

            //Act
            var res = await _presupuestosDB.SaldoDisponible("123456");

            //Assert
            Assert.Equal(estadoPresupuesto.Disponible, res[0].Disponible);
        }




    }



    
}
