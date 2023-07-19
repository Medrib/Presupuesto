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
        private PresupuestosDB _presupuestosDB = new PresupuestosDB();
        private ValidacionUsuarioDB _validacionUsuarioDB = new ValidacionUsuarioDB();

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

        //[HttpGet("/ObtenerGastosPorProducto", Name = "ObtenerGastosPorProducto")]
        //public async Task<List<Gastos>> ObtenerGastosPorProducto(string idProducto)
        //    => await _gastosDB.GastosPorProducto(idProducto);


        //[HttpGet("/ObtenerGastosTotales", Name = "ObtenerGastosTotales")]
        //public async Task<List<Gastos>> ObtenerGastosTotales()
        //    => await _gastosDB.ObtenerGastosTotales();


        //[HttpPost("/AgregarGasto", Name = "AgregarGasto")]
        //public async Task<string> AgregarGasto(AgregarGastoRequest detalle)
        //    => await _gastosDB.AgregarGasto(detalle);

        //[HttpDelete("/EliminarGasto", Name = "EliminarGasto")]
        //public async Task<string> EliminarGasto(string idGasto)
        //    => await _gastosDB.EliminarGasto(idGasto);

        [HttpGet("/GastosPorMesAño", Name = "GastosPorMesAño")]
        public async Task<List<Gastos>> GastosPorMesAño(string mesAño)
            => await _gastosDB.GastosPorMesAño(mesAño);

        [HttpDelete("/EliminarCliente",Name ="EliminarCliente")]
        public async Task<string> EliminarCliente(int dni)
            => await _datosPersonalesDB.EliminarCliente(dni);

        [HttpPost("/AgregarPresupuesto", Name = "AgregarPresupuesto")]
        public async Task<int> AgregarPresupuesto(RequestPresupuesto request)
            => await _presupuestosDB.AgregarPresupuesto(request);

        [HttpGet("/EstadoPresupuesto", Name = "EstadoPresupuesto")]
        public async Task<List<EstadoPresupuesto>> EstadoPresupuesto(string idPresupuesto)
            => await _presupuestosDB.EstadoPresupuesto(idPresupuesto);

        [HttpGet("/ValidarUsuario/{user}/{password}", Name = "ValidarUsuario")]
        public async Task<bool> ValidarUsuario(string user, string password)
            => await _validacionUsuarioDB.ValidarUsuario(user, password);
    }


}