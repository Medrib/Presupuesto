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
        public float Valor { get; set; }
        public int Consumidor { get; set; }
        public DateTime Fecha { get; set; }
    }
}
