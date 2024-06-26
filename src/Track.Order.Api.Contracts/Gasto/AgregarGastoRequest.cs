﻿using Track.Order.Domain.Entities;

namespace Track.Order.Api.Contracts.Gasto;

public class AgregarGastoRequest
{
    public DateTime? Fecha { get; set; }
    public decimal Monto { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public int IDPresupuesto { get; set; } 
    public int IDCategoriaGasto { get; set; } 
}
