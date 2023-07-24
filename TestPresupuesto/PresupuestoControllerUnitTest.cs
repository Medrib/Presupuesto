using Presupuesto.Controllers;

namespace TestPresupuesto
{
    public class PresupuestoControllerUnitTest
    {
        private readonly PresupuestoController _presupuestoController;
        public PresupuestoControllerUnitTest()
        {
            //ACA INICIALIZO LAS COSAS QUE VOY A NECESITAR EN LOS TEST DE ESTE ARCHIVO - GENERICAMENTE
            _presupuestoController =  new PresupuestoController();
        }
        [Fact]
        public async Task ValidarUsuario_Ok()
        {
            //Arrange -- ACA DECLARO Y ASIGNO LO QUE VOY A NECESITAR PARA TESTEAR ESPECIFICAMENTE PARA ESTE TEST
            var usr = "cristian";
            var pwd = "1234";

            //Act -- ACA LLAMO AL SERVICIO 
            var response = await _presupuestoController.ValidarUsuario(usr, pwd);

            //Assert -- ACA PONGO LOS ASSERT Y CONDICIONO LOS RESULTADOS.
            Assert.True(response);
        }
    }
}