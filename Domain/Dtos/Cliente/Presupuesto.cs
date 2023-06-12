using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos.Cliente
{
    public class Presupuesto
    {
        public string IdRubro { get; set; }
        public string Rubro { get; set; }
        public float Estimado { get; set; }
        public DateTime Fecha { get; set; }
    }
}
