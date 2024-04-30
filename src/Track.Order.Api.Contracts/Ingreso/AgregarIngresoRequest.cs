using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Track.Order.Api.Contracts.Ingreso
{
    public class AgregarIngresoRequest
    {
        public DateTime? Fecha { get; set; }
        public decimal Monto { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int IDPresupuesto { get; set; }
        //public Presupuesto? Presupuesto { get; set; }
        //public CategoriaGasto? CategoriaGasto { get; set; }
    }
}
