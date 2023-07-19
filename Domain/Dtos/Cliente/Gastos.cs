using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos.Cliente
{
    public class Gastos
    {
        public string Id { get; set; } //IND00000001
        public Int32 IdPresupuesto { get; set; }
        public decimal Gasto { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaCreacion { get; set; }
        public Int32 Mes { get; set; }
        public Int32 Año { get; set; }
    }

    public class AgregarGastoRequest
    {
        public int IdPresupuesto { get; set; }
        public string IdRubro { get; set; } //IND
        public decimal Valor { get; set; }
        public int Consumidor { get; set; }
    }

    public class PuedeGastarResponse
    {
        public bool PuedeGastar { get; set; }
        public decimal GastoRubro { get; set; }
    }

    public class MesAño
    {
        public Int32 Mes { get; set; }
        public Int32 Año { get; set; }
    }
}
