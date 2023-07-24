using Domain.Dtos.Cliente;
using Microsoft.AspNetCore.Mvc;
using Presupuesto.Repository;

namespace Presupuesto.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PresupuestoController : ControllerBase
    {

        private GastosDB _gastosDB = new GastosDB();
        private PresupuestosDB _presupuestosDB = new PresupuestosDB();
        private ValidacionUsuarioDB _validacionUsuarioDB = new ValidacionUsuarioDB();
    
        public PresupuestoController()
        {
        }

        [HttpPost("/AgregarGasto", Name = "AgregarGasto")]
        public async Task<string> AgregarGasto(AgregarGastoRequest detalle)
            => await _gastosDB.AgregarGasto(detalle);

        [HttpPut("/ActualizaGasto", Name = "ActualizaGasto")]
        public async Task<string> ActualizaGasto(EditarGasto detalle)
            => await _gastosDB.ActualizaGasto(detalle);

        [HttpDelete("/EliminarGasto", Name = "EliminarGasto")]
        public async Task<string> EliminarGasto(EliminaGasto idGasto)
            => await _gastosDB.EliminarGasto(idGasto);

        [HttpGet("/GastosPorMesAño", Name = "GastosPorMesAño")]
        public async Task<List<Gastos>> GastosPorMesAño(string mesAño)
            => await _gastosDB.GastosPorMesAño(mesAño);

        [HttpPost("/AgregarPresupuesto", Name = "AgregarPresupuesto")]
        public async Task<int> AgregarPresupuesto(RequestPresupuesto request)
            => await _presupuestosDB.AgregarPresupuesto(request);

        [HttpGet("/SaldoDisponible", Name = "SaldoDisponible")]
        public async Task<List<EstadoPresupuesto>> SaldoDisponible(string idPresupuesto)
            => await _presupuestosDB.SaldoDisponible(idPresupuesto);

        [HttpGet("/ValidarUsuario/{user}/{password}", Name = "ValidarUsuario")]
        public async Task<bool> ValidarUsuario(string user, string password)
            => await _validacionUsuarioDB.ValidarUsuario(user, password);

        [HttpGet("/PresupuestoPorFecha", Name = "PresupuestoPorFecha")]
        public async Task<List<PresupuestoModel>> PresupuestoPorFecha(string fecha)
          => await _presupuestosDB.PresupuestoPorFecha(fecha);
    }


}