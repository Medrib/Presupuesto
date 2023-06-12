using Domain.Dtos.Cliente;
using Microsoft.AspNetCore.Mvc;
using Presupuesto.Repository;

namespace Presupuesto.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PresupuestoController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;
        private DatosPersonalesDB _datosPersonalesDB = new DatosPersonalesDB();
        private GastosDB _gastosDB = new GastosDB();

        public PresupuestoController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet("/BuscarClientePorDni/", Name = "BuscarClientePorDni")]
        public async Task<DatosPersonales> BuscarClientePorDni(int dni)
        {
            var response = await _datosPersonalesDB.DatosPersonalesByDni(dni);
            return response;
        }

        [HttpGet("/Clientes/", Name = "Clientes")]
        public async Task<List<DatosPersonales>> Clientes()
        {
            var response = await _datosPersonalesDB.Clientes();
            return response;
        }

        [HttpGet("/ObtenerGastos", Name = "PbtenerGstos")]

        public async Task<List<Gastos>> ObtenerGastosTotales()
        {
            var response = await _gastosDB.GastosPorMes();
            return response;
        }
    }
}