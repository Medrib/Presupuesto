using Microsoft.AspNetCore.Mvc;
using Presupuesto.DataBase.Models;
using Presupuesto.Repository;

namespace Presupuesto.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PresupuestoController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;
        private DatosPersonalesDB _datosPersonalesDB = new DatosPersonalesDB();

        public PresupuestoController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet("/BuscarClientePorDni/", Name = "BuscarClientePorDni")]
        public async Task<DatosPersonales> BuscarClientePorDni(int dni)
        {
            var response =  await _datosPersonalesDB.DatosPersonalesByDni(dni);
            return response;
        }

        [HttpGet("/Clientes/", Name = "Clientes")]
        public async Task<List<DatosPersonales>> Clientes()
        {
            var response = await _datosPersonalesDB.Clientes();
            return response;
        }
    }
}