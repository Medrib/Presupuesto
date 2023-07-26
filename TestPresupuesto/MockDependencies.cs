using Moq;
using System.Data;

namespace TestPresupuesto
{
    public static class MockDependencies
    {
        public static Mock<IDbConnection> GetConnectionMock(Mock<IDataReader> readersMock, Mock<IDataParameterCollection> parametersMock)
        {
            //Mock de lo que devuleve la DB
            var readerMock = readersMock;
            readerMock.SetupSequence(_ => _.Read())
                .Returns(true)
                .Returns(false);

            //Mock del command 
            var commandMock = new Mock<IDbCommand>();
            commandMock.Setup(m => m.ExecuteReader()).Returns(readerMock.Object).Verifiable();
            commandMock.Setup(m => m.Parameters).Returns(parametersMock.Object);

            //Mock de la connection
            var connectionMock = new Mock<IDbConnection>();
            connectionMock.Setup(m => m.CreateCommand()).Returns(commandMock.Object);

            return connectionMock;
        }
    }
}
