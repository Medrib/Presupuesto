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

        [HttpGet("/ObtenerClientePorDni", Name = "ObtenerClientePorDni")]
        public async Task<DatosPersonales> ObtenerClientePorDni(int dni)
            => await _datosPersonalesDB.DatosPersonalesByDni(dni);


        [HttpGet("/ObtenerClientes", Name = "ObtenerClientes")]
        public async Task<List<DatosPersonales>> ObtenerClientes()
            => await _datosPersonalesDB.Clientes();

        [HttpGet("/ObtenerGastosPorProducto", Name = "ObtenerGastosPorProducto")]
        public async Task<List<Gastos>> ObtenerGastosPorProducto(string idProducto)
            => await _gastosDB.GastosPorProducto(idProducto);


        [HttpGet("/ObtenerGastosTotales", Name = "ObtenerGastosTotales")]
        public async Task<List<Gastos>> ObtenerGastosTotales()
            => await _gastosDB.ObtenerGastosTotales();


        [HttpPost("/AgregarGasto", Name = "AgregarGasto")]
        public async Task<string> AgregarGasto(AgregarGastoRequest detalle)
            => await _gastosDB.AgregarGasto(detalle);
    }
}