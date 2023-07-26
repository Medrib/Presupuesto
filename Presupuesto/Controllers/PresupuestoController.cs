using Domain.Dtos.Cliente;
using Microsoft.AspNetCore.Mvc;
using Presupuesto.Repository;

namespace Presupuesto.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PresupuestoController : ControllerBase
    {

        private readonly IGastosDB _gastosDB;
        private readonly IPresupuestosDB _presupuestosDB;
        private readonly IValidacionUsuarioDB _validacionUsuarioDB;
    
        public PresupuestoController(IValidacionUsuarioDB validacionUsuarioDB,
            IGastosDB gastosDB,
            IPresupuestosDB presupuestosDB
            )
        {
            _gastosDB = gastosDB;
            _presupuestosDB = presupuestosDB;
            _validacionUsuarioDB = validacionUsuarioDB;
        }

        [HttpPost("/AgregarGasto", Name = "AgregarGasto")]
        public async Task<string> AgregarGasto([FromBody] AgregarGastoRequest detalle)
            => await _gastosDB.AgregarGasto(detalle);

        [HttpPut("/ActualizaGasto", Name = "ActualizaGasto")]
        public async Task<string> ActualizaGasto([FromBody] EditarGasto detalle)
            => await _gastosDB.ActualizaGasto(detalle);

        [HttpDelete("/EliminarGasto", Name = "EliminarGasto")]
        public async Task<string> EliminarGasto([FromBody] EliminaGasto idGasto)
            => await _gastosDB.EliminarGasto(idGasto);

        [HttpGet("/GastosPorFecha/{fecha}", Name = "GastosPorFecha")]
        public async Task<List<Gastos>> GastosPorFecha([FromRoute] string fecha)
            => await _gastosDB.GastosPorMesAño(fecha);

        [HttpPost("/AgregarPresupuesto", Name = "AgregarPresupuesto")]
        public async Task<int> AgregarPresupuesto([FromBody] RequestPresupuesto request)
            => await _presupuestosDB.AgregarPresupuesto(request);

        [HttpGet("/SaldoDisponible/{idPresupuesto}", Name = "SaldoDisponible")]
        public async Task<List<EstadoPresupuesto>> SaldoDisponible([FromRoute] string idPresupuesto)
            => await _presupuestosDB.SaldoDisponible(idPresupuesto);

        [HttpGet("/ValidarUsuario/{user}/{password}", Name = "ValidarUsuario")]
        public async Task<bool> ValidarUsuario([FromRoute] string user, [FromRoute] string password)
            => await _validacionUsuarioDB.ValidarUsuario(user, password);

        [HttpGet("/PresupuestoPorFecha/{fecha}", Name = "PresupuestoPorFecha")]
        public async Task<List<PresupuestoModel>> PresupuestoPorFecha([FromRoute] string fecha)
          => await _presupuestosDB.PresupuestoPorFecha(fecha);
    }


}