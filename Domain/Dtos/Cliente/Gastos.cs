

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
        public int IdPresupuesto { get; set; } //123456
        public string IdRubro { get; set; } //IND
        public decimal Gasto { get; set; }
        public string Usuario { get; set; }
    }

    public class PuedeGastarResponse
    {
        public bool PuedeGastar { get; set; }
        public decimal GastoRubro { get; set; }
    }

    public class EditarGasto
    {
        public string Id { get; set; } //IND00000001
        public int IdPresupuesto { get; set; }
        public decimal Gasto { get; set; }
        public string Usuario { get; set;}

    }
    public class EliminaGasto
    {
        public string Id { get; set; } //IND00000001
        public string Usuario { get; set; }

    }
}
