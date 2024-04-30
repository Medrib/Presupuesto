using System.Globalization;

namespace Track.Order.Domain.Entities;

public class CategoriaGasto
{
    public int IDCategoriaGasto { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = null!;
}
