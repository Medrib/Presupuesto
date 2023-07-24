using System.ComponentModel.DataAnnotations;


namespace Domain.Dtos.Cliente
{
    public class PresupuestoModel
    {
        [Key]
        public int Id { get; set; }
        public int IdPresupuesto { get; set; }//100000, 999999
        public string IdRubro { get; set; }
        public string Rubro { get; set; }
        public string Usuario { get; set; }
        public decimal Presupuesto { get; set; }
        public decimal Gastado { get; set; }
        public DateTime FechaDeCreacion { get; set; }
        public Int32 Anio { get; set; }
        public Int32 Mes { get; set; }
    }

    public class RequestPresupuesto
    {
        public string Usuario { get; set; }
        public int DuracionPresupuesto { get; set; }

        public List<DetallePresupuesto> detallePresupuesto { get; set;}

    }

    public class DetallePresupuesto
    {

        public string IdRubro { get; set; }
        public string Rubro { get; set; }
        public decimal Presupuesto { get; set; }
    
    }

    public class EstadoPresupuesto
    {
        public string Rubro { get; set; }
        public decimal Disponible { get; set; }
    }

}
