
using System.ComponentModel.DataAnnotations;

namespace Track.Order.Domain.Entities
{
    public class Gastos
    {
        [Key]
        public int IDGasto { get; set; }
        public decimal Monto { get; set; }
        public DateTime? Fecha { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int IDPresupuesto { get; set; } // Clave externa a la entidad Presupuesto
        public int IDCategoriaGasto { get; set; } // Clave externa a la entidad CategoriaGasto
        public Presupuesto? Presupuesto { get; set; }
        public CategoriaGasto? CategoriaGasto { get; set; }
    }
}
