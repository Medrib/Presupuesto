namespace Track.Order.Infrastructure;

using Track.Order.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class TrackOrderDbContext : DbContext
{


    public TrackOrderDbContext(DbContextOptions<TrackOrderDbContext> options)
        : base(options)
    {
    }

    public DbSet<Gastos> Gasto=> Set<Gastos>();
    public DbSet<CategoriaGasto> categoriaGasto => Set<CategoriaGasto>();
    public DbSet<Presupuesto> Presupuesto => Set<Presupuesto>();
    public DbSet<Ingresos> Ingreso => Set<Ingresos>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TrackOrderDbContext).Assembly);
    }

}
