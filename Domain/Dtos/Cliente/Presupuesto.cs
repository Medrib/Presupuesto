using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos.Cliente
{
    public class PresupuestoModel
    {
        [Key]
        public int Id { get; set; }
        public string IdPresupuesto { get; set; }
        public string IdRubro { get; set; }
        public string Rubro { get; set; }
        public int Responsable { get; set; }
        public decimal Estimado { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }

    public class RequestPresupuesto
    {
        public int Responsable { get; set; }
        public int DuracionPresupuesto { get; set; }

        public List<DetallePresupuesto> detallePresupuesto { get; set;}

    }

    public class DetallePresupuesto
    {

        public string IdRubro { get; set; }
        public string Rubro { get; set; }
        public decimal Estimado { get; set; }

    }
}
