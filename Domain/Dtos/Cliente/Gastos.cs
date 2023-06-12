using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos.Cliente
{
    public class Gastos
    {
        public string Id { get; set; }
        public decimal Valor { get; set; }
        public Int32 Consumidor { get; set; }
        public DateTime Fecha { get; set; }
    }

    public class AgregarGastoRequest
    {
        public string IdRubro { get; set; }
        public decimal Valor { get; set; }
        public int Consumidor { get; set; }
    }
}
